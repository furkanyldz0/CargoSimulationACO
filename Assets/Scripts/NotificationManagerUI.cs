using DG.Tweening;
using TMPro;
using UnityEngine;

public class NotificationManagerUI : MonoBehaviour
{
    public static NotificationManagerUI Instance { private set; get; }

    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private Transform tweenBottomStartPositionTransform;
    [SerializeField] private Transform tweenTopEndPositionTransform;

    private Sequence mySequence;

    private void Awake() {
        if(Instance != null) {
            Debug.LogError("Sahnede birden fazla NotificationManager nesnesi var!");
        }
        Instance = this;
    }

    private void Start() {
        notificationText.transform.position = tweenBottomStartPositionTransform.position;

        Tween textTween = notificationText.transform
            .DOMove(tweenTopEndPositionTransform.position, 2f)
            .SetEase(Ease.OutCirc);

        mySequence = DOTween.Sequence();
        mySequence.SetAutoKill(false);

        mySequence.Append(textTween)
            .Insert(1f, notificationText.DOFade(0f, 1.5f).SetEase(Ease.InQuad))
            .OnComplete(() => {
                notificationText.gameObject.SetActive(false);
                //notificationText.transform.position = tweenBottomStartPositionTransform.position;
                //tekrar konumunu manuel sýfýrlamama gerek yok restart atarken aklýnda tutuyor
            });

        //mySequence.Pause();
    }

    public void Notificate(string warningMessage) {
        notificationText.text = warningMessage;
        notificationText.gameObject.SetActive(true);

        PlayTextAnimation();
    }

    private void PlayTextAnimation() {
        mySequence.Restart();
    }

    private void OnDestroy() {
        // AutoKill kapattýðýmýz için obje silinirken belleði temizlemeyi unutmamalýyýz
        mySequence?.Kill();
    }
}
