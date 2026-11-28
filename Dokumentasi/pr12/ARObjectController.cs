using UnityEngine;

public class ARObjectController : MonoBehaviour
{
    [Header("Target AR")]
    public Transform targetObject;
    public string targetTag = "ARObject";

    [Tooltip("Jika aktif dan objek bertag punya child, child pertama yang diputar/diskala. " +
             "Ini mencegah rotasi ditimpa oleh AR Tracked Image Manager.")]
    public bool useFirstChildAsModel = true;

    [Header("Rotation")]
    public float rotateStep = 15f;

    [Header("Scale")]
    public float scaleFactor = 1.15f;
    public float minScale = 0.2f;
    public float maxScale = 3.0f;

    [Header("Optional Feedback")]
    public AudioSource audioSource;

    void Start()
    {
        FindTargetIfNeeded();
    }

    public void FindTargetIfNeeded()
    {
        if (targetObject != null) return;

        GameObject found = GameObject.FindGameObjectWithTag(targetTag);
        if (found != null)
        {
            if (useFirstChildAsModel && found.transform.childCount > 0)
                targetObject = found.transform.GetChild(0);
            else
                targetObject = found.transform;
        }
    }

    private bool TargetReady()
    {
        if (targetObject == null)
        {
            FindTargetIfNeeded();
        }

        if (targetObject == null)
        {
            Debug.LogWarning("Target AR belum ditemukan. Scan marker terlebih dahulu.");
            return false;
        }

        return true;
    }

    public void RotateLeft()
    {
        if (!TargetReady()) return;
        targetObject.Rotate(0f, -rotateStep, 0f, Space.Self);
        PlayFeedback();
    }

    public void RotateRight()
    {
        if (!TargetReady()) return;
        targetObject.Rotate(0f, rotateStep, 0f, Space.Self);
        PlayFeedback();
    }

    public void ScaleUp()
    {
        if (!TargetReady()) return;
        ApplyScale(scaleFactor);
        PlayFeedback();
    }

    public void ScaleDown()
    {
        if (!TargetReady()) return;
        ApplyScale(1f / scaleFactor);
        PlayFeedback();
    }

    private void ApplyScale(float factor)
    {
        Vector3 current = targetObject.localScale;
        float currentX = Mathf.Max(current.x, 0.0001f);
        float desiredX = Mathf.Clamp(currentX * factor, minScale, maxScale);
        float ratio = desiredX / currentX;
        targetObject.localScale = current * ratio;
    }

    private void PlayFeedback()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
