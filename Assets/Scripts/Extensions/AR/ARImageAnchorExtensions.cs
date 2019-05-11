using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class ARImageAnchorExtensions : MonoBehaviour
{
    private const int UNDEFINED_SCORE = -1;

    [SerializeField]
    private List<ARReferenceImage> _referenceImages = new List<ARReferenceImage>();

    private List<string> _imageNames = new List<string>();

    [SerializeField]
    private List<GameObject> _generateObjectPrefabs = new List<GameObject>();

    private List<GameObject> _generateObjects = new List<GameObject>();

    [SerializeField]
    private bool isNotTrackingToUnactive = true;

    [SerializeField]
    private bool isRemoveToDestroy = true;

    private void Awake()
    {
        UnityARSessionNativeInterface.ARImageAnchorAddedEvent += AddImageAnchor;
        UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent += UpdateImageAnchor;
        UnityARSessionNativeInterface.ARImageAnchorRemovedEvent += RemoveImageAnchor;

        foreach (ARReferenceImage image in _referenceImages)
        {
            _imageNames.Add(image.imageName);
            _generateObjects.Add(null);
        }
    }

    // マーカー検知
    private void AddImageAnchor(ARImageAnchor anchor)
    {
        int index = GetImageAnchorIndex(anchor);
        if (index == UNDEFINED_SCORE)
        {
            return;
        }

        Vector3 position = UnityARMatrixOps.GetPosition(anchor.transform);
        Quaternion quaternion = UnityARMatrixOps.GetRotation(anchor.transform);

        _generateObjects[index] = Instantiate(_generateObjectPrefabs[index], position, quaternion);
    }

    // マーカー更新
    private void UpdateImageAnchor(ARImageAnchor anchor)
    {
        int index = GetImageAnchorIndex(anchor);
        if (index == UNDEFINED_SCORE)
        {
            return;
        }

        GameObject obj = _generateObjects[index];

        if (anchor.isTracked)
        {
            if (isNotTrackingToUnactive && !obj.activeSelf)
            {
                obj.SetActive(true);
            }
            obj.transform.position = UnityARMatrixOps.GetPosition(anchor.transform);
            obj.transform.rotation = UnityARMatrixOps.GetRotation(anchor.transform);
        }
        else if (isNotTrackingToUnactive && obj.activeSelf)
        {
            obj.SetActive(false);
        }
    }

    // マーカー解放
    private void RemoveImageAnchor(ARImageAnchor anchor)
    {
        int index = GetImageAnchorIndex(anchor);
        if (index == UNDEFINED_SCORE)
        {
            return;
        }

        if (_generateObjects[index] != null)
        {
            Destroy(_generateObjects[index].gameObject);
            _generateObjects[index] = null;
        }
    }

    // 登録されているマーカーのインデックスを取得
    private int GetImageAnchorIndex(ARImageAnchor anchor)
    {
        if (!_imageNames.Contains(anchor.referenceImageName))
        {
            return UNDEFINED_SCORE;
        }

        return _imageNames.FindIndex((e) => { return e == anchor.referenceImageName; });
    }

    private void OnDestroy()
    {
        UnityARSessionNativeInterface.ARImageAnchorAddedEvent -= AddImageAnchor;
        UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent -= UpdateImageAnchor;
        UnityARSessionNativeInterface.ARImageAnchorRemovedEvent -= RemoveImageAnchor;
    }
}
