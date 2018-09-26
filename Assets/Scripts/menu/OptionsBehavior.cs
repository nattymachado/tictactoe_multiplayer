using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class OptionsBehavior : MonoBehaviour {

    private Dropdown _gameModeDropdown;
    private Dropdown _difficultyDropdown;
    private Text _difficultyText;
    private Button _startButton;
    private BoardConfiguration _configuration;
    
    private string _whoStartSceneName = "WhoStartScene";
    private string _networkSceneName = "ConfigurationNetwork";
    private string _nextSceneName = "";
    private string _optionsSceneName = "OptionsScene";

    private void Start () {

        _nextSceneName = _whoStartSceneName;
        _gameModeDropdown = GameObject.Find("GameModeDropdown").GetComponent<Dropdown>();
        List<Dropdown.OptionData> optionDataList = new List<Dropdown.OptionData>();
        optionDataList.Add(new Dropdown.OptionData("Select a Game Mode ..."));
        for (int position=0; position<GameModeOptions.options.Length; position++)
        {
            optionDataList.Add(new Dropdown.OptionData(GameModeOptions.options[position].Label));
        }
        _gameModeDropdown.AddOptions(optionDataList);
        _gameModeDropdown.onValueChanged.AddListener(delegate {
            GameModeDropdownChanged(_gameModeDropdown);
        });

        _difficultyDropdown = GameObject.Find("DifficultyDropdown").GetComponent<Dropdown>();
        _difficultyDropdown.onValueChanged.AddListener(delegate {
            DifficultyDropdownChanged(_difficultyDropdown);
        });

        _difficultyDropdown.gameObject.SetActive(false);
        _difficultyText = GameObject.Find("DifficultyText").GetComponent<Text>();
        _difficultyText.gameObject.SetActive(false);

        _startButton = GameObject.Find("StartButton").GetComponent<Button>();
        _startButton.onClick.AddListener(delegate {
            StartGame(_startButton);
        });

        _configuration = BoardConfigurationGetter.getConfigurationObject();
        _configuration.EnabledGeneralAudio();
        _configuration.Difficulty = (DifficultyOptions.Options) 0;

    }

    private void GameModeDropdownChanged(Dropdown dropdown)
    {
        if (dropdown.value != 0)
        {
            if ((dropdown.value) == GameModeOptions.options[0].Value)
            {
                _difficultyDropdown.gameObject.SetActive(true);
                _difficultyText.gameObject.SetActive(true);
                _nextSceneName = _whoStartSceneName;
            }
            else
            {
                _difficultyDropdown.gameObject.SetActive(false);
                _difficultyText.gameObject.SetActive(false);
                _nextSceneName = _networkSceneName;
            }
            _configuration.GameModeOption = new GameModeOption(dropdown.options[dropdown.value].text, dropdown.value);
        } else
        {
            _difficultyDropdown.gameObject.SetActive(false);
            _difficultyText.gameObject.SetActive(false);
            _configuration.GameModeOption = null;
        }
    }

    private void DifficultyDropdownChanged(Dropdown dropdown)
    {
        _configuration.Difficulty = (DifficultyOptions.Options) dropdown.value;
    }

    private void StartGame(Button startButton)
    {
        if (_configuration.GameModeOption != null)
        {
            Debug.Log(_nextSceneName);
            StartCoroutine(SceneLoader.LoadScene(_nextSceneName));
            StartCoroutine(SceneLoader.UnloadScene(_optionsSceneName));
        }
        
    }
}

