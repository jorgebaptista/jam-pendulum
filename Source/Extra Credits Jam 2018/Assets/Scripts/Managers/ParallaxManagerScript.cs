using UnityEngine;

public class ParallaxManagerScript : MonoBehaviour
{
    [System.Serializable]
    private struct ParallaxBG
    {
        public Transform backgroundTransform;
        public float parallaxScale;
    }

    [SerializeField]
    private ParallaxBG[] backgrounds;

    [SerializeField]
    private float smoothing = 1f;

    private Transform cameraTransform;
    private Vector3 previousCamPosition;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        previousCamPosition = cameraTransform.position;
    }

    private void Update()
    {
        foreach (ParallaxBG background in backgrounds)
        {
            float parallaxX = (previousCamPosition.x - cameraTransform.position.x) * background.parallaxScale;
            float parallaxY = (previousCamPosition.y - cameraTransform.position.y) * background.parallaxScale;

            float backgroundTargetX = background.backgroundTransform.position.x + parallaxX;
            float backgroundTargetY = background.backgroundTransform.position.y + parallaxY;

            Vector3 backgroundTargetPos = new Vector3(backgroundTargetX, backgroundTargetY, background.backgroundTransform.position.z);

            background.backgroundTransform.position = Vector3.Lerp(background.backgroundTransform.position, backgroundTargetPos, smoothing * Time.deltaTime);
        }

        previousCamPosition = cameraTransform.position;
    }
}
