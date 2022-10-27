using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Astar
{
    public class AstarCellMagnifier 
    {
        private readonly float offsetZ = -5.0f;

        private readonly AstarMap map;
        private AstarCellMagnifierBehaviour cellMagnifier;

        public AstarCellMagnifier(ref AstarMap map, AstarCellMagnifierBehaviour cellMagnifier)
        {
            this.map = map;
            this.cellMagnifier = cellMagnifier;
        }

        public void AddListener(AstarCellMagnifierBehaviour magnifier)
        {
            magnifier.OnMagnifying += OnMagnifying;
        }

        public void RemoveListener(AstarCellMagnifierBehaviour magnifier)
        {
            magnifier.OnMagnifying -= OnMagnifying;
        }

        private void OnMagnifying(AstarNode node, bool toggle)
        {
            var cell = cellMagnifier.gameObject.GetComponent<AstarCellRenderer>();
            if (toggle)
            {
                cell.AssignNode(node);

                //todo: node position (row, col) where col == x, row == y
                cell.transform.localPosition = new Vector3(node.Position.y, -node.Position.x, offsetZ);

                var scale = map.Size / 10;
                cell.transform.localScale = new Vector3(scale, scale, 1.0f);
            }
            else
            {
                cell.AssignNode(null);
            }

            cellMagnifier.gameObject.SetActive(toggle);
        }
    }
}
