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
    public Sprite defaultCrawlSprite;  // 未解锁时显示的图片

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
        menuCanvas.SetActive(false);
        UpdateUI();
    }

    public void ToggleMenu()
    {
        bool isMenuActive = menuCanvas.activeSelf;
        menuCanvas.SetActive(!isMenuActive);
        Time.timeScale = isMenuActive ? 1 : 0;

        if (!isMenuActive)
        {
            UpdateUI();  // 打开菜单时更��UI
        }
    }

    private void UpdateUI()
    {
        // 更新碎片数量
        int fragmentCount = PlayerManager.Instance.GetFragmentCount();
        fragmentCountText.text = $"收集到的碎片: {fragmentCount}";

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
        if (image == null) return;

        bool isUnlocked = PlayerManager.Instance.HasLearnedCrawlType(crawlType);
        image.sprite = isUnlocked ? image.sprite : defaultCrawlSprite;
        image.color = isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);  // 未解锁时显示灰色
    }
} 