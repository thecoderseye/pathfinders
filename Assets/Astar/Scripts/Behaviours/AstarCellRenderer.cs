using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Text;
using System.Runtime.CompilerServices;

namespace Astar
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class AstarCellRenderer : MonoBehaviour
    {
        public TextMeshPro scoreG;
        public TextMeshPro scoreH;
        public TextMeshPro scoreF;

        private SpriteRenderer renderer;
        private AstarNode node;

        public AstarNode Node => node;

        private bool isDirty;


        public Vector3 positionOrigin;
        public Vector3 scaleOrigin;

        private Collider2D collider;
        public AstarCellRenderer()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            positionOrigin = transform.localPosition;
            scaleOrigin = transform.localScale;

            renderer = gameObject.GetComponent<SpriteRenderer>();
            collider = gameObject.GetComponent<Collider2D>();

            Clear();
        }

        // Update is called once per frame
        void Update()
        {
            if (node != null && isDirty)
            {
                if (node.ScoreF > 0)
                {
                    scoreG.text = $"g{node.ScoreG}";
                    scoreH.text = $"h{node.ScoreH}";
                    scoreF.text = $"f{node.ScoreF}";
                }
                else
                //if (node.Type == NodeType.Wall || node.Status.Value.Equals(NodeStatusLiteral.None))
                {
                    scoreG.text = "";
                    scoreH.text = "";
                    scoreF.text = "";
                }
                renderer.color = node.Status.GetColor();
                isDirty = false;
            }
        }

        public void AssignNode(AstarNode node)
        {
            this.node = node;
            if (node != null)
            {
                node.OnValueChanged += OnValueChanged;
                isDirty = true;
            }
            else
            {
                Clear();
            }

        }

        private void Clear()
        {
            scoreG.text = "";
            scoreH.text = "";
            scoreF.text = "";
        }

        private void OnValueChanged()
        {
            isDirty = true;
        }
    }
}
