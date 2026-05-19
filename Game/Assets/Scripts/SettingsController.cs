using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown difficultyDropdown; // Dropdown de seleccion de dificultad
    [SerializeField] private Button saveButton; // Botón de guardado de preferencias
    [SerializeField] private PanelManager panelManager;

    public List<Selectable> _formFields; // Almaceno los gameObjects que quiero deshabilitar durante la actualización de los ajustes del usuario
    public GameObject loadingSpinner;

    private DifficultyOption[] _difficulties; // Array para almacenar los niveles de dificultad obtenidos desde el API.
    // private int _savedDifficultyId;

    /// <summary>
    /// Al iniciar la primera vez carga las dificultades desde el API y las guarda en la sesión para evitar futuras llamadas
    /// </summary>
    private async void Start()
    {
        await LoadDifficulties();
    }

    /// <summary>
    /// Carga las dificultades desde la sesión o, si no están, desde el API. Luego las muestra en el dropdown y selecciona la dificultad guardada en las preferencias del usuario.
    /// Si no se pudieron cargar las dificultades, deja el dropdown vacío.
    /// Si el usuario no tiene preferencias guardadas, no selecciona ninguna dificultad.
    /// Si el usuario tiene preferencias pero la dificultad guardada ya no existe, tampoco selecciona ninguna dificultad.
    /// </summary>
    /// <returns></returns>
    private async Task LoadDifficulties()
    {
        // 1. Primero comprueba si hay niveles de dificultad guardadas en sesión.
        if (UserSession.Instance.Difficulties != null)
        {
            _difficulties = UserSession.Instance.Difficulties;
        }
        else
        {
            // 1.1 Si no las hubiera, va a la API para obtenerlas de BBDD.
            // Si no obtiene nada deja el dropdown vacio.
            // Si las obtiene, las guarda en sesión para futuras consultas.
            loadingSpinner.SetActive(true);
            _difficulties = await UserSession.Instance.FetchDifficulties();
            loadingSpinner.SetActive(false);

            if (_difficulties == null || _difficulties.Length == 0) return;
            UserSession.Instance.SetDifficulties(_difficulties);
        }

        // 2. Limpia el dropdown y lo rellena con las niveles obtenidos.
        difficultyDropdown.ClearOptions();
        var options = new List<TMP_Dropdown.OptionData>();
        foreach (var d in _difficulties)
            options.Add(new TMP_Dropdown.OptionData(d.name));
        difficultyDropdown.AddOptions(options);

        // 3. Selecciona la dificultad guardada en las preferencias del usuario, si existe.
        // Si no tuviera, marcaremos por defecto la primera dificultad (opción 0), pero sin guardar nada en preferencias hasta que el usuario pulse "Guardar".
        var prefs = UserSession.Instance.User?.preferences;
        if (prefs == null)
        {
            difficultyDropdown.value = 0;
            return;
        }

        // _savedDifficultyId = prefs.idDifficulty;
        int index = Array.FindIndex(_difficulties, d => d.id == prefs.idDifficulty);
        if (index >= 0) difficultyDropdown.value = index;
    }

    /// <summary>
    /// Al pulsar el botón de guardar los ajustes del usuario, valida si se ha modificado alguno de los valores que ya tuviera el usuario guardados
    /// así evitamos hacer llamadas innecesarias a la API. Si se ha modificado la dificultad, hace la llamada al API para actualizar las preferencias del usuario. Si la llamada es correcta, actualiza el valor guardado en sesión para futuras comparaciones.
    /// </summary>
    public async void OnSaveClicked()
    {
        var prefs = UserSession.Instance?.User?.preferences;
        if (prefs == null || _difficulties == null) return;

        int selectedId = _difficulties[difficultyDropdown.value].id;
        if (selectedId == prefs.idDifficulty) return;

        loadingSpinner.SetActive(true);
        SetFieldsInteractable(false);

        saveButton.interactable = false;
        bool success = await UpdatePreferences(prefs.id, selectedId);
        saveButton.interactable = true;
        SetFieldsInteractable(true);
        loadingSpinner.SetActive(false);

        if (!success) return;

        panelManager.ShowToast("¡Preferencias actualizadas!", "Tus ajustes se han guardado correctamente.");
        UserSession.Instance.UpdateUserPreferences(selectedId, _difficulties[difficultyDropdown.value].name);
    }

    // /// <summary>
    // /// Obtiene la lista de dificultades desde el API. Si la llamada falla, devuelve null.
    // /// </summary>
    // /// <returns>Niveles de dificultad obtenidos</returns>
    // private async Task<DifficultyOption[]> FetchDifficulties()
    // {
    //     using var req = UnityWebRequest.Get(ApiConfig.Difficulty.GetAll);
    //     var op = req.SendWebRequest();
    //     while (!op.isDone) await Task.Yield();

    //     if (req.result != UnityWebRequest.Result.Success) return null;

    //     var response = JsonUtility.FromJson<DifficultiesResponse>(req.downloadHandler.text);
    //     return response?.difficulties;
    // }

    private async Task<bool> UpdatePreferences(int preferencesId, int idDifficulty)
    {
        var payload = new UpdatePreferencesRequest { idDifficulty = idDifficulty };
        var json = JsonUtility.ToJson(payload);

        using var req = new UnityWebRequest($"{ApiConfig.User.Preferences}/{preferencesId}", "PUT");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        var op = req.SendWebRequest();
        while (!op.isDone) await Task.Yield();

        return req.result == UnityWebRequest.Result.Success;
    }

    /// <summary>
    /// Habilita o deshabilita los campos de entrada y botones para evitar múltiples interacciones
    /// </summary>
    /// <param name="interactable"> = true -> habilitado, = false -> deshabilitado</param>
    private void SetFieldsInteractable(bool interactable)
    {
        foreach (var field in _formFields)
            field.interactable = interactable;
    }
}
