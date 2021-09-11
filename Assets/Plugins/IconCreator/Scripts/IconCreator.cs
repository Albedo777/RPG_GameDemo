#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[SerializableAttribute]
public class OutlineColor {
    public Color color;
    public string colorName;
}

public class IconCreator : MonoBehaviour {

    [HeaderAttribute ("Icon parameters")]
    [SerializeField]
    private int IconSizeX;
    public int iconSizeX {
        get {
            return doubleQuality ? IconSizeX * 2 : IconSizeX;
        }
        set {
            cam.pixelRect = new Rect (0, 0, iconSizeX, iconSizeY);
        }
    }
    [SerializeField]
    private int IconSizeY;
    public int iconSizeY {
        get {
            return doubleQuality ? IconSizeY * 2 : IconSizeY;
        }
        set {
            cam.pixelRect = new Rect (0, 0, iconSizeX, iconSizeY);
        }
    }

    [HeaderAttribute ("Double Quality")]
    public bool doubleQuality;

    [HeaderAttribute ("Model List")]
    public GameObject[] modelsToCreateIconsFrom;

    private Transform Pivot;
    public Transform pivot {
        get {
            if (Pivot != null) {
                return Pivot;
            }
            else {
                return Pivot = transform.Find ("Pivot");
            }
        }
        set {
            if (Pivot != null) {
                Pivot = value;
            }
            else {
                Pivot = transform.Find ("Pivot");
            }
        }
    }
    [HeaderAttribute ("Pivot transform")]
    [SerializeField]
    private Vector3 PivotPosition;
    public Vector3 pivotPosition {
        get {
            return pivot.transform.position;
        }
        set {
            pivot.transform.position = value;
        }
    }
    [SerializeField]
    private Vector3 PivotRotation;
    public Vector3 pivotRotation {
        get {
            return pivot.transform.eulerAngles;
        }
        set {
            pivot.transform.eulerAngles = value;
        }
    }
    [SerializeField]
    private Vector3 PivotScale;
    public Vector3 pivotScale {
        get {
            return pivot.transform.localScale;
        }
        set {
            pivot.transform.localScale = value;
        }
    }
    [HeaderAttribute ("Outline parameters")]
    public bool useOutline;
    public int outlineSize;
    public OutlineColor[] outlineColors;
    public bool blurOutline;
    public int blurSize;
    public int blurIterations;

    [HeaderAttribute ("Bottom fade")]
    public bool fadeBottom;
    public int fadeBottomSize;

    [HeaderAttribute ("Background/foreground")]
    [SerializeField]
    private bool UseBackground;
    public bool useBackground {
        get {
            return UseBackground;
        }
        set {
            background.gameObject.SetActive (value);
        }
    }
    [SerializeField]
    private bool UseForeground;
    public bool useForeground {
        get {
            return UseForeground;
        }
        set {
            foreground.gameObject.SetActive (value);
        }
    }

    [HeaderAttribute ("Offset correction")]
    public Vector2 offsetCorrection;

    private Camera Cam;
    public Camera cam {
        get {
            if (Cam != null) {
                return Cam;
            }
            else {
                return Cam = GetComponent<Camera> ();
            }
        }
    }
    public GameObject currentModel { get; set; }
    public OutlineColor currentOutlineColor { get; set; }

    private Transform Background;
    public Transform background {
        get {
            if (Background != null) {
                return Background;
            }
            else {
                return Background = transform.Find ("Background");
            }
        }
        set {
            Background = transform.Find ("Background");
        }
    }

    private Transform Foreground;
    public Transform foreground {
        get {
            if (Foreground != null) {
                return Foreground;
            }
            else {
                return Foreground = transform.Find ("Foreground");
            }
        }
        set {
            Foreground = transform.Find ("Foreground");
        }
    }

    private Rect RenderRect;
    public Rect renderRect {
        get {
            return new Rect (offsetCorrection.x, offsetCorrection.y, iconSizeX + offsetCorrection.x, iconSizeY + offsetCorrection.y);
        }
    }

    public Texture2D foregroundLayer { get; set; }
    public Texture2D backgroundLayer { get; set; }

    private int CurrentIndex;
    public int currentIndex {
        get {
            return CurrentIndex;
        }
        set {
            CurrentIndex = value;
            if (CurrentIndex < 0) {
                CurrentIndex = 0;
            }
            if (CurrentIndex >= modelsToCreateIconsFrom.Length - 1) {
                CurrentIndex = modelsToCreateIconsFrom.Length - 1;
            }
            while (pivot.childCount != 0) {
                DestroyImmediate (pivot.GetChild (0).gameObject);
            }
            if (modelsToCreateIconsFrom.Length > 0) {
                GameObject model = Instantiate (modelsToCreateIconsFrom [CurrentIndex]) as GameObject;
                model.transform.SetParent (pivot);
                model.transform.localPosition = Vector3.zero;
                model.transform.localScale = Vector3.one;
                model.transform.eulerAngles = PivotRotation;
            }
        }
    }

    private int Count;
    public int count {
        get {
            return Count;
        }
        set {
            Count = value;
            if (count != 0) {
                EditorUtility.DisplayProgressBar ("Icon Creator", "Creating icon number " + count + " of " + modelsToCreateIconsFrom.Length, count / ((float)modelsToCreateIconsFrom.Length));
            }
            else {
                EditorUtility.ClearProgressBar ();
            }
        }
    }

    public IEnumerator CreateIcons() {

        count = 0;

        while (pivot.childCount != 0) {
            DestroyImmediate (pivot.GetChild (0).gameObject);
        }

        Background.gameObject.SetActive (false);
        Foreground.gameObject.SetActive (false);

        yield return new WaitForEndOfFrame ();

        cam.pixelRect = new Rect (0, 0, iconSizeX, iconSizeY);

        if (useBackground) {
            background.gameObject.SetActive (true);
            backgroundLayer = new Texture2D (iconSizeX, iconSizeY);
            Render (backgroundLayer);
            background.gameObject.SetActive (false);
        }
        if (useForeground) {
            foreground.gameObject.SetActive (true);
            foregroundLayer = new Texture2D (iconSizeX, iconSizeY);
            Render (foregroundLayer);
            foreground.gameObject.SetActive (false);
        }

        if (modelsToCreateIconsFrom.Length == 0) {
            EditorUtility.DisplayDialog ("No models found!", "You must have at least one model in the models to create icons from list.", "Ok");
        }
        else if(useOutline && outlineColors.Length == 0) {
            EditorUtility.DisplayDialog("No outline colours!", "When using outline you must at least have 1 outline colour.", "Ok");
        }
        else {
            for (int j = 0; j < modelsToCreateIconsFrom.Length; j++) {
                count++;
                InstantiateModel (modelsToCreateIconsFrom [j]);
                yield return new WaitForEndOfFrame ();
                CreateIcon ();
                while (pivot.childCount != 0) {
                    DestroyImmediate (pivot.GetChild (0).gameObject);
                }
            }
        }


        yield return new WaitForEndOfFrame ();

        useBackground = useBackground;
        useForeground = useForeground;
        currentIndex = currentIndex;

        System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
        System.Type type = assembly.GetType ("UnityEditor.InspectorWindow");
        EditorWindow inspectorWindow = EditorWindow.GetWindow (type);
        inspectorWindow.Focus ();

        AssetDatabase.Refresh ();

        count = 0;
    }

    public void InstantiateModel(GameObject model) {
        currentModel = Instantiate (model) as GameObject;
        currentModel.name = model.name;
        currentModel.transform.SetParent (pivot);
        currentModel.transform.localScale = Vector3.one;
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.eulerAngles = pivotRotation;
        currentModel.SetActive (true);
    }

    public void CreateIcon() {
        Texture2D icon = new Texture2D (iconSizeX, iconSizeY);
        icon.hideFlags = HideFlags.DontSave;
        Texture2D outline = new Texture2D (iconSizeX, iconSizeY);
        outline.hideFlags = HideFlags.DontSave;
        RenderIcon (icon);
        if (useOutline) {
            if(currentOutlineColor == null) {
                currentOutlineColor = new OutlineColor();
                currentOutlineColor.color = Color.white;
            }
            outline = OutlineIcon (icon);
            outline.Apply();
        }
        Texture2D combined = new Texture2D (iconSizeX, iconSizeY);
        combined.hideFlags = HideFlags.DontSave;

        for (int i = 0; i < outlineColors.Length; i++) {
            currentOutlineColor = outlineColors [i];
            currentOutlineColor.color.a = 1;
            Texture2D tempOutline = outline;
            tempOutline.hideFlags = HideFlags.DontSave;
            SetOutlineColor (tempOutline);
            combined = CombineLayers (icon, tempOutline);
            if (doubleQuality) {
                SaveIcon (HalfSize (combined));
            }
            else {
                SaveIcon (combined);
            }
        }
    }

    public void SetOutlineColor(Texture2D texture) {
        Color color = currentOutlineColor.color;
        for (int x = 0; x < iconSizeX; x++) {
            for (int y = 0; y < iconSizeY; y++) {
                color.a = texture.GetPixel (x, y).a;
                texture.SetPixel (x, y, color);
            }
        }
    }

    public void RenderIcon(Texture2D icon) {
        Render (icon);
    }

    public void Render(Texture2D layer) {
        cam.Render ();
        layer.ReadPixels (renderRect, 0, 0);
    }

    public Texture2D OutlineIcon(Texture2D icon) {

        Color32[] colors = icon.GetPixels32 ();
        Color32[] outlineColors = new Color32[colors.Length];

        for (int i = 0; i < outlineColors.Length; i++) {
            if (colors [i].a >= 1f) {
                for (int j = 0; j < outlineSize; j++) {
                    if ((i + j) % iconSizeX < iconSizeX && (i + j) % iconSizeX > outlineSize) {
                        if (i + j < outlineColors.Length) {
                            outlineColors [i + j] = currentOutlineColor.color;
                        }
                    }
                    if ((i - j) % iconSizeX < iconSizeX - outlineSize ) {

                        if (i - j > 0) {
                            outlineColors [i - j] = currentOutlineColor.color;
                        }
                    }
                    if (i - j * iconSizeX > 0) {
                        outlineColors [i - j * iconSizeX] = currentOutlineColor.color;
                    }
                    if (i + j * iconSizeX < outlineColors.Length) {
                        outlineColors [i + j * iconSizeX] = currentOutlineColor.color;
                    }
                }
            }
        }

        Texture2D outline = new Texture2D (iconSizeX, iconSizeY);
        outline.hideFlags = HideFlags.DontSave;
        outline.SetPixels32 (outlineColors);
        if (blurOutline) {
            outline = BlurOutline (outline);
        }

        return outline;
    }

    public Texture2D BlurOutline(Texture2D outline) {
        return Blur.FastBlur (outline, blurSize, blurIterations);
    }

    public Texture2D FadeBottom(Texture2D icon) {
        Color32[] colors = icon.GetPixels32 ();
        float firstFoundIndex = Mathf.Infinity;
        for (int i = 0; i < colors.Length; i++) {
            if (colors [i].a != 0) {
                if (firstFoundIndex == Mathf.Infinity) {
                    firstFoundIndex = (int)(i / iconSizeX);
                }
            }
        }

        for (int i = iconSizeX * (int)firstFoundIndex; i < colors.Length; i++) {
            if (i < iconSizeX * firstFoundIndex + iconSizeX ) {
                for (int j = 0; j < fadeBottomSize; j++) {
                    if (i + iconSizeX * j < colors.Length && colors [i + iconSizeX * j].a != 0) {
                        float a = (j / (float)fadeBottomSize) * 255;
                        float alpha = System.Math.Min (a, colors [i + iconSizeX * j].a);
                        colors [i + iconSizeX * j].a = (byte)Mathf.Clamp (alpha, 0, 255);
                    }
                }
            }
        }
        icon.SetPixels32 (colors);
        return icon;
    }

    public Texture2D HalfSize(Texture2D icon) {
        Texture2D halfSize = new Texture2D (iconSizeX / 2, iconSizeY / 2);
        halfSize.hideFlags = HideFlags.DontSave;
        halfSize.SetPixels (icon.GetPixels (1));
        halfSize.Apply ();
        return halfSize;
    }

    public Texture2D CombineLayers(Texture2D icon, Texture2D outline) {
        Texture2D combined = new Texture2D (iconSizeX, iconSizeY, TextureFormat.ARGB32, true);
        combined.hideFlags = HideFlags.DontSave;

        Color32[] combinedPixels = new Color32[iconSizeX * iconSizeY];
        Color32[] backgroundLayerPixels = useBackground ? backgroundLayer.GetPixels32 () : new Color32[0];
        Color32[] foregroundLayerPixels = useForeground ? foregroundLayer.GetPixels32 () : new Color32[0];
        Color32[] iconLayerPixels = icon.GetPixels32 ();
        Color32[] outlineLayerPixels = useOutline ? outline.GetPixels32 () : new Color32[0];

        if (useOutline) {
            Color32[] outlinedIcon = outlineLayerPixels;
            for (int j = 0; j < iconLayerPixels.Length; j++) {
                if (iconLayerPixels [j].a != 0) {
                    //We use color32.Lerp to make sure we mix the color of the background and the outline properly.
                    //The (float)outlineLayerPixels[j].a/(float)255 part make sure that the alpha of the outline mix properly with the background
                    outlinedIcon [j] = iconLayerPixels [j];
                    outlinedIcon [j].a = 255;
                }
            }
            iconLayerPixels = outlinedIcon;
            if (fadeBottom) {
                icon.SetPixels32 (iconLayerPixels);
            }
        }

        if (fadeBottom) {
            icon = FadeBottom (icon);
            iconLayerPixels = icon.GetPixels32 ();
        }

        if (useBackground) {
            //Run through all the pixels in the background layer and add them to the combined texture
            for (int j = 0; j < backgroundLayerPixels.Length; j++) {
                if (backgroundLayerPixels [j].a != 0) {
                    combinedPixels [j] = backgroundLayerPixels [j];
                }
            }
        }

        //Run through all the pixels in the icon layer
        for (int j = 0; j < iconLayerPixels.Length; j++) {
            //New add the pixels of the actual model on top of the background and outline
            if (iconLayerPixels [j].a != 0) {
                combinedPixels [j] = combinedPixels [j] = Color32.Lerp (combinedPixels [j], iconLayerPixels [j], ((float)iconLayerPixels [j].a) / (float)255);
                if (useBackground) {
                    combinedPixels [j].a = 255;
                }
            }
        }

        //Run through all the pixels in the foreground layer and add them to the combined texture
        if (useForeground) {
            for (int j = 0; j < foregroundLayerPixels.Length; j++) {
                if (foregroundLayerPixels [j].a != 0) {
                    //Again we use Lerp to make sure the colors get mixed properly and also to make sure that if the foreground uses alpha to mix it properly
                    combinedPixels [j] = Color32.Lerp (combinedPixels [j], foregroundLayerPixels [j], (float)foregroundLayerPixels [j].a / (float)255);
                    combinedPixels [j].a = 255;
                }
            }
        }

        combined.SetPixels32 (combinedPixels);

        combined.Apply ();

        return combined;
    }

    public void SaveIcon(Texture2D icon) {
        if (!Directory.Exists (Application.dataPath + "/GeneratedIcons/" + (doubleQuality ? iconSizeX / 2 : iconSizeX).ToString () + "x" + (doubleQuality ? iconSizeY / 2 : iconSizeY).ToString () + "/" + (useOutline ? currentOutlineColor.colorName : "Transparent"))) {
            Directory.CreateDirectory (Application.dataPath + "/GeneratedIcons/" + (doubleQuality ? iconSizeX / 2 : iconSizeX).ToString () + "x" + (doubleQuality ? iconSizeY / 2 : iconSizeY).ToString () + "/" + (useOutline ? currentOutlineColor.colorName : "Transparent"));
        }
        File.WriteAllBytes (Path.Combine (Application.dataPath + "/GeneratedIcons/" + (doubleQuality ? iconSizeX / 2 : iconSizeX).ToString () + "x" + (doubleQuality ? iconSizeY / 2 : iconSizeY).ToString () + "/" + (useOutline ? currentOutlineColor.colorName : "Transparent"), currentModel.name + ".png"), icon.EncodeToPNG ());
        EditorUtility.ClearProgressBar ();
    }

    public void OnValidate() {
        pivotPosition = PivotPosition;
        pivotRotation = PivotRotation;
        pivotScale = PivotScale;
        useBackground = UseBackground;
        useForeground = UseForeground;
        iconSizeX = IconSizeX;
        iconSizeY = IconSizeY;
    }
}

#endif
