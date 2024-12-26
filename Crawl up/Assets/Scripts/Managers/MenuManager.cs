using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("UI引用")]
    public GameObject menuCanvas;
    public TextMeshProUGUI fragmentCountText;
    
    [Header("爬行类型图片")]
    public Image basicCrawlImage;
    public Image geckoCrawlImage;
    public Image turtleCrawlImage;
    public Image snakeCrawlImage;
    public Image catCrawlImage;
    public Image chameleonCrawlImage;
    public Sprite defaultCrawlSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (menuCanvas != null)
            menuCanvas.SetActive(false);
            
        // 添加空检查
        if (PlayerManager.Instance != null)
        {
            UpdateUI();
        }
    }

    public void ToggleMenu()
    {
        if (menuCanvas == null) return;

        bool isMenuActive = menuCanvas.activeSelf;
        menuCanvas.SetActive(!isMenuActive);
        Time.timeScale = isMenuActive ? 1 : 0;

        if (!isMenuActive && PlayerManager.Instance != null)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (PlayerManager.Instance == null) return;

        // 更新碎片数量
        if (fragmentCountText != null)
        {
            int fragmentCount = PlayerManager.Instance.GetFragmentCount();
            fragmentCountText.text = $"收集到的碎片: {fragmentCount}";
        }

        // 更新爬行类型图片
        UpdateCrawlTypeImage(basicCrawlImage, "Basic");
        UpdateCrawlTypeImage(geckoCrawlImage, "Gecko");
        UpdateCrawlTypeImage(turtleCrawlImage, "Turtle");
        UpdateCrawlTypeImage(snakeCrawlImage, "Snake");
        UpdateCrawlTypeImage(catCrawlImage, "Cat");
        UpdateCrawlTypeImage(chameleonCrawlImage, "Chameleon");
    }

    private void UpdateCrawlTypeImage(Image image, string crawlType)
    {
        if (image == null || PlayerManager.Instance == null) return;

        bool isUnlocked = PlayerManager.Instance.HasLearnedCrawlType(crawlType);
        if (isUnlocked)
        {
            image.color = Color.white;
        }
        else if (defaultCrawlSprite != null)
        {
            image.sprite = defaultCrawlSprite;
            image.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }
} 