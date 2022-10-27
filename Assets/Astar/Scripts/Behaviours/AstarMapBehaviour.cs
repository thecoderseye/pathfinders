using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Util;
using static UnityEngine.Random;

namespace Astar
{
    public class AstarMapBehaviour : MonoBehaviour
    {
        [Flags]
        public enum RedrawOption
        {
            None = 0,
            StartPosition = 1 << 0,
            EndPosition = 1 << 1,
            ResizeMap = 1 << 2,
            All = StartPosition | EndPosition | ResizeMap,
        }

        //public AstarMapConfig Config;
        public AstarCellRenderer CellPrototype;
        public AstarCellMagnifierBehaviour CellMagnifier;

        public GraphicRaycaster Raycaster;

        public AstarMap Map => map;

        private AstarMap map;
        private GameObject mapObject;
        private AstarCellRenderer[,] cellRenderers;

        private RedrawOption redrawOptions;

        private bool isDirty => (redrawOptions & RedrawOption.All) > 0;

        private AstarCellMagnifier magnifier;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            redrawOptions = RedrawOption.None;
            yield return NewMap();
        }

        public void Clear()
        {
            using (var raycaster = new EventRaycastSwitch(ref Raycaster))
            {
                map.Clear();
                AstarAlgorithm.GetInstance().Clear();
            }
        }

        public void Reset(int size = -1)
        {
            AstarAlgorithm.GetInstance().Reset();
            if (size < 0)
                size = map.Size;

            StartCoroutine(NewMap(size));
        }

        private IEnumerator NewMap(int size = 10)
        {
            using (var raycaster = new EventRaycastSwitch(ref Raycaster))
            {
                yield return Cleanup(size);

                map = new AstarMap(size);
                magnifier = new AstarCellMagnifier(ref map, CellMagnifier);

                yield return map.Build();
                yield return PopulateNodes();
            }
        }

        private IEnumerator ResetCamera(int newSize)
        {
            var camera = Camera.main;
            var orthographicSize = newSize / 2;
            camera.orthographicSize = orthographicSize;

            var position = camera.transform.localPosition;
            position.x = orthographicSize - 0.5f;
            position.y = 0.5f - orthographicSize;
            camera.transform.localPosition = position;

            yield return null;
        }

        private IEnumerator Cleanup(int newSize)
        {
            if (mapObject != null && map != null)
            {
                for (var row = map.Size - 1; row >= 0; --row)
                {
                    var child = mapObject.transform.GetChild(row);

                    for (var col = 0; col < map.Size; ++col)
                    {
                        var node = child.GetChild(col);
                        var magnifierBehaviour = node.gameObject.GetComponent<AstarCellMagnifierBehaviour>();
                        magnifier.RemoveListener(magnifierBehaviour);
                    }

                    child.gameObject.SetActive(false);
                    GameObject.Destroy(child.gameObject, 0.01f);

                    yield return null;
                }

                GameObject.Destroy(mapObject);
                magnifier = null;
                mapObject = null;
            }

            yield return new WaitForEndOfFrame();

            yield return ResetCamera(newSize);
            yield return new WaitForEndOfFrame();
        }

        private IEnumerator PopulateNodes()
        {
            //yield return Cleanup();
            var size = map.Size;

            mapObject = new GameObject();
            mapObject.name = $"[{map.Size}x{map.Size}]";
            mapObject.transform.parent = transform;

            cellRenderers = new AstarCellRenderer[map.Size, map.Size];
            for (var row = 0; row < size; ++row)
            {
                var instance = new GameObject();
                instance.name = $"row[{row}]";
                instance.transform.parent = mapObject.transform;
                instance.transform.localPosition = Vector3.zero;

                for (var col = 0; col < size; ++col)
                {
                    var node = map.Matrix[row, col];
                    var nodeRenderer = GameObject.Instantiate<AstarCellRenderer>(CellPrototype, Vector3.zero, Quaternion.identity, instance.transform);

                    nodeRenderer.name = $"[{row},{col}]";
                    nodeRenderer.transform.localPosition = new Vector2(col, -row);
                    nodeRenderer.gameObject.SetActive(true);
                    nodeRenderer.AssignNode(node);

                    var magnifierBehaviour = nodeRenderer.gameObject.AddComponent<AstarCellMagnifierBehaviour>();
                    magnifier.AddListener(magnifierBehaviour);

                    cellRenderers[row, col] = nodeRenderer;
                }

                yield return null;
            }
        }


        // Update is called once per frame
        void Update()
        {
            if (isDirty)
            {
                redrawOptions = RedrawOption.None;
            }
        }

        public void OnPickUpStartPosition()
        {
            redrawOptions |= RedrawOption.StartPosition;
        }

        public void OnPickUpEndPosition()
        {
            redrawOptions |= RedrawOption.EndPosition;
        }

        public void OnResizeMap(int option)
        {
            var size = map.Size;
            switch (option)
            {
                case 0:
                    size = 10;
                    break;

                case 1:
                    size = 20;
                    break;

                case 2:
                    size = 40;
                    break;
            }

            if (size == map.Size)
                return;

            Reset(size);
            redrawOptions |= RedrawOption.ResizeMap;
        }

        private void PostExit(ref AstarAlgorithm algorithm, ref IAstarAlgorithmState state)
        {
            var status = state.GetStatus();
            var pathCount = algorithm.CountPath();
            var openCount = algorithm.CountOpenSet();
            var closeCount = algorithm.CountCloseSet();

            var message = new StringBuilder();
            message.AppendLine();
            message.AppendLine($"{openCount + closeCount + pathCount} determined in total");
            message.Append($"({openCount} opened, {closeCount} closed)");

            if ((status & AstarAlgorithm.Status.Found) > 0)
            {
                message.Insert(0, $"Path exists: {pathCount} nodes");
            }
            else if ((status & AstarAlgorithm.Status.NotFound) > 0)
            {
                message.Insert(0, "Path does not exists.");
            }

            new Alert(message.ToString(), 3.0f);
        }

        public IEnumerator StartFindingPath(AstarMapBehaviour mapBehaviour)
        {
            var algorithm = AstarAlgorithm.GetInstance();
            var state = algorithm as IAstarAlgorithmState;

            using (var raycaster = new EventRaycastSwitch(ref Raycaster))
            {
                var status = state.GetStatus();
                if ((status & AstarAlgorithm.Status.Exit) > 0)
                {
                    algorithm?.Clear();
                    mapBehaviour?.Clear();
                }

                var heuristicSelector = GameObject.FindObjectOfType<AstarHeuristicSelector>();
                Assert.IsNotNull(heuristicSelector);

                var dataStructureSelector = GameObject.FindObjectOfType<AstarDataStructureSelector>();
                Assert.IsNotNull(dataStructureSelector);

                var directionSelector = GameObject.FindObjectOfType<AstarDirectionSelector>();
                Assert.IsNotNull(directionSelector);

                IAstarPriorityQueue queue;
                var type = dataStructureSelector.GetSelectedType();
                switch (type)
                {
                    case AstarDataStructureSelector.Type.LIFO:
                        queue = new AstarPriorityQueueLIFO();
                        break;

                    case AstarDataStructureSelector.Type.FIFO:
                    default:
                        queue = new AstarPriorityQueueFIFO();
                        break;

                }

                algorithm?.Setup(Map, queue, heuristicSelector.GetSelectedHeuristic(), directionSelector.GetDirection());
                yield return algorithm.StartFindingPath();

                if ((state.GetStatus() & AstarAlgorithm.Status.Exit) > 0)
                    PostExit(ref algorithm, ref state);
            }
        }

        public void StepForward(AstarMapBehaviour mapBehaviour)
        {
            var algorithm = AstarAlgorithm.GetInstance();
            var algorithmState = algorithm as IAstarAlgorithmState;
            using (var raycaster = new EventRaycastSwitch(ref Raycaster))
            {
                var status = algorithmState.GetStatus();

                switch (status)
                {
                    case AstarAlgorithm.Status.Exit:
                    case AstarAlgorithm.Status.ExitWithFound:
                    case AstarAlgorithm.Status.ExitWithNotFound:
                        mapBehaviour?.Clear();
                        algorithm?.Clear();
                        goto case AstarAlgorithm.Status.NotStarted;

                    case AstarAlgorithm.Status.NotStarted:
                        var heuristicSelector = GameObject.FindObjectOfType<AstarHeuristicSelector>();
                        Assert.IsNotNull(heuristicSelector);

                        var dataStructureSelector = GameObject.FindObjectOfType<AstarDataStructureSelector>();
                        Assert.IsNotNull(dataStructureSelector);

                        var directionSelector = GameObject.FindObjectOfType<AstarDirectionSelector>();
                        Assert.IsNotNull(directionSelector);

                        IAstarPriorityQueue queue;
                        var type = dataStructureSelector.GetSelectedType();
                        switch (type)
                        {
                            case AstarDataStructureSelector.Type.LIFO:
                                queue = new AstarPriorityQueueLIFO();
                                break;

                            case AstarDataStructureSelector.Type.FIFO:
                            default:
                                queue = new AstarPriorityQueueFIFO();
                                break;

                        }
                        algorithm?.Setup(Map, queue, heuristicSelector.GetSelectedHeuristic(), directionSelector.GetDirection());
                        break;
                }

                var endStatus = algorithm.StepForward();
                if ((endStatus & AstarAlgorithm.Status.Exit) > 0)
                    PostExit(ref algorithm, ref algorithmState);
            }
        }
    }
}
