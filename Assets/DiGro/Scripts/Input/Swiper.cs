
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

using DiGro.Input;

public class Swiper : MonoBehaviour {

    [Header("Linked objects")]
    //[SerializeField] private Collider2D swiperCollider = null;
    [SerializeField] private InputHandler m_handler = null;
    [SerializeField] private List<SpriteRenderer> directionLines = new List<SpriteRenderer>();

    [Header("Settings")]
    [SerializeField] [Range(0, 360)] private int swipeSectors = 6;
    [SerializeField] private float sectorsOffset = 0;
    public bool showSwipeLine = true;
    public bool printLog = false;
    [SerializeField] private bool breakingMode = true;
    [SerializeField] private float m_AxisIdling = 0.001f;
    [Header("Test")]
    [SerializeField] private bool printSectorsTest = false;
    [SerializeField] private float sectorsTestDelta = 5f;

    private Action<DiGro.Input.EventData> action;

    private Vector2 startPosition = Vector2.zero;
    private int currentSector = 0;
    private float currentAngle = 0;
    private Vector2 direction = Vector2.zero;
    private bool isOnOperate = false;

    //private InputEvent swipeEvent;


    // Collider2D внутри которого будут смотреться события свайпа.
    //public Collider2D Collider { set { swiperCollider = value; } }

    // Количество секторов, на которые будет разбито впространство. 
    // Во время события свайпа будет определен угол этого свайпа и к какому сектору он относится.
    // 0-ой сектор начинается с 0 градусов.
    public int Sectors { set { swipeSectors = value; } }

    // Смещение 0-го сектора. Пример: -0.5 - на на пол сектора влево, 3 - на 3 сектора вправа.
    public float SectorsOffset { set { sectorsOffset = value; } }

    // Действие которое бедут вызвано, когда событие свайпа активно.
    public Action<DiGro.Input.EventData> Action { set { action = value; } }
    public Vector2 StartPosition { get { return startPosition; } }

    // Режим с прерыванием - true - после старта свайпа при первом же собитии движения свайпа, 
    //  событие будет завершено без ожидания события окончания свайпа.
    // Режим без прерываний - false - событие будет продолжаться до события окончания свайпа.
    public bool BreakingMode {
        get { return breakingMode; }
        set { breakingMode = value; }
    }

    // Сектор, в котором находится вторая точка свайпа.
    public int Sector { get { return currentSector; } }

    // Угол, на который повернута вторая точка свайпа относительно первой.
    public float Angle { get { return currentAngle; } }

    // Нормализованный вектор направления свайпа. 
    public Vector2 Direction { get { return direction; } }


    private void Awake() {
        if (!m_handler)
            Debug.LogError("Not all set in " + GetType());

        m_handler.BeginDragListener += OnBeginDrag;
        m_handler.DragListener += OnDrag;
        m_handler.EndDragListener += OnEndDrag;
    }

    private void OnDestroy() {
        m_handler.BeginDragListener -= OnBeginDrag;
        m_handler.DragListener -= OnDrag;
        m_handler.EndDragListener -= OnEndDrag;
    }

    private void Update() {
        if (printSectorsTest) {
            printSectorsTest = false;
            TestSectors(sectorsTestDelta);
        }
    }

    private void TestSectors(float angleDelta) {
        string str = "";
        for (float g = 0; g <= 360; g += angleDelta) {
            int sector = 0;
            if (swipeSectors != 0) {
                float sectorSize = 360 / swipeSectors;
                float offset = sectorSize * sectorsOffset;
                for (int j = 0; j < swipeSectors; j++)
                    if (g >= j * sectorSize + offset && g < j * sectorSize + offset + sectorSize) {
                        sector = j;
                        str += "angle: " + g.ToString() + " sector: " + sector.ToString() + "\n";
                        break;
                    }
            }
        }
        Debug.Log(str);
    }

    private void SetDelta(Vector2 delta) {
        float g = Vector2.Angle(Vector2.up, delta);
        if (delta.x < 0)
            g = 360 - g;
        currentAngle = g;
        direction = delta.normalized;
        currentSector = 0;
        if (swipeSectors != 0) {
            float sectorSize = 360 / swipeSectors;
            float offset = sectorSize * sectorsOffset;
            for (int i = 0; i < swipeSectors; i++)
                if (g >= i * sectorSize + offset && g < i * sectorSize + offset + sectorSize) {
                    currentSector = i;
                    break;
                }
        }
    }

    private void PrintLog() {
        Debug.Log("angle: " + currentAngle + " direction: " + direction + " sector: " + currentSector);
    }

    private void ShowDirectionLines() {
        if (directionLines.Count == 0 || directionLines.Count < swipeSectors)
            return;

        for (int i = 0; i < directionLines.Count; i++) {
            if (i != Sector)
                directionLines[i].enabled = false;
            else
                directionLines[i].enabled = true;
        }
    }

    private void HideDirectionLines() {
        for (int i = 0; i < directionLines.Count; i++)
            directionLines[i].enabled = false;
    }

    private void OnBeginDrag(DiGro.Input.EventData ev) {
        isOnOperate = true;
        startPosition = ev.worldPosition;
    }

    private void OnDrag(DiGro.Input.EventData ev) {
        var delta = ev.worldPosition - ev.pressWorldPosition;
        if (Math.Abs(delta.x) > m_AxisIdling || Math.Abs(delta.y) > m_AxisIdling) {
            if (isOnOperate) {
                SetDelta(delta);
                if (showSwipeLine)
                    ShowDirectionLines();
                if (printLog)
                    PrintLog();
                action?.Invoke(ev);
                if (breakingMode)
                    isOnOperate = false;
            }
        }
    }

    private void OnEndDrag(DiGro.Input.EventData ev) {
        isOnOperate = false;
        HideDirectionLines();
    }

}
