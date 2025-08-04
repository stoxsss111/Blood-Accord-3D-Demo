using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MiniPCController : MonoBehaviour
{
    [Header("References")]
    public GameObject miniPCPanel;
    public GameObject passwordPanel;
    public GameObject mailPanel;
    public GameObject mailContentPanel;

    public TMP_InputField passwordInput;
    public TextMeshProUGUI wrongPasswordText;
    public Button enterButton;
    public Button closePCButton;

    public Transform senderListParent;
    public GameObject senderButtonPrefab; // Кнопка с Image, TMP_Text, Background
    public TextMeshProUGUI mailContentText;
    public GameObject attachmentPanel;    // Панель для показа вложения (может быть просто Image+Text+Button)
    public TextMeshProUGUI attachmentNameText;
    public Button attachmentOpenButton;
    public Button closeMailButton;

    [Header("Database")]
    public MailDatabase mailDatabase;

    private bool unlocked = false;
    private int lastMailIndex = -1;
    private List<GameObject> senderButtons = new List<GameObject>();

    // --- Саморефлексия ---
    // Теперь код более “живой”: появилось состояние для PlayerPrefs, логика выделения кнопки,
    // а для вложений — доп.панель и кнопка для демонстрации.

    private void Start()
    {
        miniPCPanel.SetActive(false);
        passwordPanel.SetActive(false);
        mailPanel.SetActive(false);
        mailContentPanel.SetActive(false);
        wrongPasswordText.gameObject.SetActive(false);

        enterButton.onClick.AddListener(OnPasswordEntered);
        closePCButton.onClick.AddListener(ClosePC);
        closeMailButton.onClick.AddListener(CloseMailContent);
        attachmentOpenButton.onClick.AddListener(OpenAttachment);

        // 1. Проверяем PlayerPrefs (был ли уже введён пароль)
        unlocked = PlayerPrefs.GetInt("PCUnlocked", 0) == 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !miniPCPanel.activeSelf)
        {
            OpenPC();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && miniPCPanel.activeSelf)
        {
            ClosePC();
        }
        // 4. Реакция на Enter для ввода пароля
        if (passwordPanel.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            OnPasswordEntered();
        }
    }

    void OpenPC()
    {
        miniPCPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (unlocked)
        {
            ShowMail();
        }
        else
        {
            passwordPanel.SetActive(true);
            passwordInput.text = "";
            wrongPasswordText.gameObject.SetActive(false);
            mailPanel.SetActive(false);

            // Ставим фокус на InputField — удобно!
            passwordInput.ActivateInputField();
        }
    }

    public void OnPasswordEntered()
    {
        if (passwordInput.text == mailDatabase.mainPassword)
        {
            unlocked = true;
            PlayerPrefs.SetInt("PCUnlocked", 1);
            passwordPanel.SetActive(false);
            ShowMail();
        }
        else
        {
            wrongPasswordText.gameObject.SetActive(true);
        }
    }

    void ShowMail()
    {
        mailPanel.SetActive(true);
        passwordPanel.SetActive(false);
        mailContentPanel.SetActive(false);

        // Очищаем старый список кнопок отправителей
        foreach (var btn in senderButtons)
            Destroy(btn);
        senderButtons.Clear();

        // Динамически создаём кнопки
        for (int i = 0; i < mailDatabase.mails.Count; i++)
        {
            var mail = mailDatabase.mails[i];
            var btnObj = Instantiate(senderButtonPrefab, senderListParent);
            var btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = mail.sender;

            // Индекс для захвата в анонимную функцию
            int capturedIndex = i;
            btnObj.GetComponent<Button>().onClick.AddListener(() => ShowMailContent(capturedIndex));

            senderButtons.Add(btnObj);

            // Обнуляем фон (например, делаем белым)
            SetButtonHighlight(btnObj, false);
        }
    }

    void ShowMailContent(int mailIndex)
    {
        var mail = mailDatabase.mails[mailIndex];
        mailContentText.text = mail.body;
        mailContentPanel.SetActive(true);
        lastMailIndex = mailIndex;

        // 2. Подсветка активного отправителя
        for (int i = 0; i < senderButtons.Count; i++)
        {
            SetButtonHighlight(senderButtons[i], i == mailIndex);
        }

        // 3. Вложения (если есть)
        if (mail.hasAttachment)
        {
            attachmentPanel.SetActive(true);
            attachmentNameText.text = mail.attachmentName;
            attachmentOpenButton.gameObject.SetActive(true);
        }
        else
        {
            attachmentPanel.SetActive(false);
        }
    }

    void SetButtonHighlight(GameObject btnObj, bool highlight)
    {
        // Пример через смену цвета Image
        var img = btnObj.GetComponent<Image>();
        if (img != null)
            img.color = highlight ? new Color(0.7f, 0.85f, 1.0f, 1f) : Color.white;
    }

    public void CloseMailContent()
    {
        mailContentPanel.SetActive(false);
        // 2. Подсветку сбрасываем? Можно оставить как есть, если удобно
    }

    void ClosePC()
    {
        miniPCPanel.SetActive(false);
        passwordPanel.SetActive(false);
        mailPanel.SetActive(false);
        mailContentPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OpenAttachment()
    {
        // Демонстрация: просто выводим лог
        var mail = mailDatabase.mails[lastMailIndex];
        Debug.Log($"Открыто вложение: {mail.attachmentName}");

        // Можно тут добавить свою логику: например, показать картинку, текст или “секретное” окно
    }
}
