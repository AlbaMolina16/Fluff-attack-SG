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
    [Header("Dificultad")]
    [SerializeField] private TMP_Dropdown difficultyDropdown; // Dropdown de seleccion de dificultad

    [Header("Parámetros de la dificultad")]
    public TMP_InputField gameTimeInput;
    public TMP_InputField spawnFrequencyInput;
    public TMP_InputField lifeTimeInput;
    public TMP_InputField speedInput;

    [Header("Boton guardar")]
    [SerializeField] private Button saveButton; // Botón de guardado de preferencias
    [SerializeField] private PanelManager panelManager;

    public List<Selectable> _formFields; // Almaceno los gameObjects que quiero deshabilitar durante la actualización de los ajustes del usuario
    public GameObject loadingSpinner;

    private DifficultyOption[] _difficulties; // Array para almacenar los niveles de dificultad obtenidos desde el API.

    /// <summary>
    /// Al iniciar la primera vez carga las dificultades desde el API y las guarda en la sesión para evitar futuras llamadas
    /// </summary>
    private async void Start()
    {
        await LoadDifficulties();
    }

    public void OnEnable()
    {
        // Nos suscribimos al evento de cambio del dropdown para aplicar los valores por defecto diseñados
        difficultyDropdown.onValueChanged.AddListener(SetDifficultyDefaultValues);
        // Se van a actualizar los campos del panel con los parámetros de dificultad que tiene asignados el usuario en sesión
        LoadDifficultParameters();
    }

    public void OnDisable()
    {
        difficultyDropdown.onValueChanged.RemoveListener(SetDifficultyDefaultValues);
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
        InitDifficultyDropdown();
        // 3. Carga el valor de los parámetros definidos por el usuario
        LoadDifficultParameters();
    }

    private void InitDifficultyDropdown()
    {
        difficultyDropdown.ClearOptions();
        var options = new List<TMP_Dropdown.OptionData>();
        foreach (var d in _difficulties)
            options.Add(new TMP_Dropdown.OptionData(d.name));
        difficultyDropdown.AddOptions(options);
    }


    private void LoadDifficultParameters()
    {
        if (UserSession.Instance.UserDifficulty != null && _difficulties != null)
        {
            var difficulty = UserSession.Instance.UserDifficulty;

            int index = Array.FindIndex(_difficulties, d => d.id == difficulty.id);
            if (index >= 0)
            {
                difficultyDropdown.value = index;
                SetFieldsDefaultValues(difficulty);
            }
        }
    }

    private void SetFieldsDefaultValues(DifficultyOption difficulty)
    {
        gameTimeInput.text = difficulty.gameTime.ToString();
        spawnFrequencyInput.text = difficulty.spawnRate.ToString();
        lifeTimeInput.text = difficulty.enemyLifeTime.ToString();
        speedInput.text = difficulty.enemySpeed.ToString();
        SetInteractableParamatersByDifficulty(difficulty.name);
    }

    private void SetDifficultyDefaultValues(int index)
    {
        SetFieldsDefaultValues(_difficulties[index]);
    }

    private void SetInteractableParamatersByDifficulty(string difficultyName)
    {
        // Si la dificultad es la fácil, deshabilitamos el campo de velocidad porque no se aplica en esta dificultad
        if (difficultyName == "easy")
        {
            speedInput.interactable = false;
            lifeTimeInput.interactable = true;
            spawnFrequencyInput.interactable = true;
        }
        else if (difficultyName == "autoadaptative")
        {
            // En el modo de juego autoadaptativo sólo se puede modificar el tiempo de partida
            speedInput.interactable = false;
            lifeTimeInput.interactable = false;
            spawnFrequencyInput.interactable = false;
        }
        else
        {
            speedInput.interactable = true;
            lifeTimeInput.interactable = true;
            spawnFrequencyInput.interactable = true;
        }
    }

    /// <summary>
    /// Al pulsar el botón de guardar los ajustes del usuario, valida si se ha modificado alguno de los valores que ya tuviera el usuario guardados
    /// así evitamos hacer llamadas innecesarias a la API. Si se ha modificado la dificultad, hace la llamada al API para actualizar las preferencias del usuario. Si la llamada es correcta, actualiza el valor guardado en sesión para futuras comparaciones.
    /// </summary>
    public async void OnSaveClicked()
    {
        var prefs = UserSession.Instance?.UserDifficulty;
        if (prefs == null || _difficulties == null) return;

        int selectedId = _difficulties[difficultyDropdown.value].id;
        // Si no ha cambiado nada entonces no hacemos nada para evitar llamadas a la API
        // y no actualizamos el valor guardado en sesión para evitar futuras comparaciones erróneas.
        if (selectedId == prefs.id
            && prefs.gameTime == float.Parse(gameTimeInput.text)
            && prefs.enemyLifeTime == float.Parse(lifeTimeInput.text)
            && prefs.enemySpeed == float.Parse(speedInput.text)
            && prefs.spawnRate == float.Parse(spawnFrequencyInput.text)) return;

        loadingSpinner.SetActive(true);
        SetFieldsInteractable(false);
        saveButton.interactable = false;

        DifficultyOption updatedDifficultyParameters = new()
        {
            id = selectedId,
            name = _difficulties[difficultyDropdown.value].name,
            gameTime = float.Parse(gameTimeInput.text),
            enemyLifeTime = float.Parse(lifeTimeInput.text),
            enemySpeed = float.Parse(speedInput.text),
            spawnRate = float.Parse(spawnFrequencyInput.text),
            amountEnemies = _difficulties[difficultyDropdown.value].amountEnemies, // No se puede modificar este valor desde el panel de ajustes, así que lo dejamos igual que el de la dificultad seleccionada
            movements = _difficulties[difficultyDropdown.value].movements // No se puede modificar
        };

        // Si ha modificado la dificultad, llamamos a la API para que permisista en futuras sesiones cuando se conecte
        if (selectedId != prefs.id)
        {
            bool success = await UpdatePreferences(selectedId);
        }

        saveButton.interactable = true;
        SetFieldsInteractable(true);
        SetInteractableParamatersByDifficulty(updatedDifficultyParameters.name);
        loadingSpinner.SetActive(false);

        try
        {
            UserSession.Instance.UpdateUserPreferences(updatedDifficultyParameters);
            panelManager.ShowToast("¡Preferencias actualizadas!", "Tus ajustes se han guardado correctamente.");
        }
        catch (System.Exception ex)
        {
            panelManager.ShowToast("Error al actualizar preferencias", "No se han podido guardar tus ajustes. Inténtalo de nuevo.");
            Debug.LogError($"Error al actualizar las preferencias del usuario: {ex.Message}");
        }
    }

    private async Task<bool> UpdatePreferences(int idDifficulty)
    {
        var payload = new UpdatePreferencesRequest { idDifficulty = idDifficulty };
        var json = JsonUtility.ToJson(payload);

        var userId = UserSession.Instance.User.id;
        using var req = new UnityWebRequest($"{ApiConfig.User.Preferences}/{userId}", "PUT");
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
