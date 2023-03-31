using System;
using System.Collections.Generic;
using AnimFlex;
using AnimFlex.Essentials;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.DefenderSpawn {
    public class MaterialModify : MonoBehaviour {

        public Group group;
        public bool sharedMaterial = true;
        public List<ModItem> mods = new();
        
        public enum Group {
            A, B, C, D, E, F, G
        }

        public void ApplyModIfMatch(Group group) {
            if (this.group != group) return;
            Apply();
        }

        [Button]
        public void Apply() {
            if (TryGetComponent<Renderer>( out var rend )) {
                for (int i = 0; i < mods.Count; i++) {
                    mods[i].Apply( sharedMaterial ? rend.sharedMaterial : rend.material );
                }
            }
        }


        [Serializable]
        public class ModItem {

            public enum FieldType {
                Color, Float, Int
            }

            public string fieldName;
            
            public FieldType fieldType = FieldType.Color;

            [ShowIf(nameof(fieldType), FieldType.Color), LabelText("Value")]
            public Color colorValue;

            [ShowIf(nameof(fieldType), FieldType.Float), LabelText("Value")]
            public float floatValue;

            [ShowIf(nameof(fieldType), FieldType.Int), LabelText("Value")]
            public int intValue;
            
            
            public void Apply(Material mat) {
                switch (fieldType) {
                    case FieldType.Color:
                        mat.SetColor( fieldName, colorValue );
                        break;
                    case FieldType.Float:
                        mat.SetFloat( fieldName, floatValue );
                        break;
                    case FieldType.Int:
                        mat.SetInteger( fieldName, intValue );
                        break;
                }
            }
        }
    }
}