namespace OSCollider
{
    using System.Collections.Generic;
    using UnityEngine;
    #if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(ConeCollider))]
    public class BtnEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Update"))
            {
                ConeCollider cone =target as ConeCollider;
                cone.UpdateDivision();
             
            }
        }
    }
    #endif
    
    public class ConeCollider : MonoBehaviour
    {
        [Tooltip("コライダーの分割数")] [SerializeField]private int Div = 16; //number of Division
        [Tooltip("上の半径")] [SerializeField] private float TopR = 2; //TopRadius
        [Tooltip("下の半径")] [SerializeField] private float BottomR = 1; //BottomRadius
        [Tooltip("コーンの高さ")] [SerializeField] private float Height = 2; //Height
        [Tooltip("ボックスの厚み")] [SerializeField] private float Width = 0.2f; //Width
        [SerializeField] private bool isDebug;
        [SerializeField] private GameObject attachObject;
        [SerializeField] PhysicMaterial CollidersMaterial;

        private string colliderName = "Collider:";
        private List<BoxCollider> bclist = new List<BoxCollider>();

        private void Awake()
        {
#if UNITY_EDITOR
            BoxCollider[] cols = GetComponentsInChildren<BoxCollider>();
            foreach (var collider in cols)
            {
                DestroyImmediate(collider.gameObject);
            }
#endif
            GenerateCone();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (isDebug)
            {
                Vector3[] tp = new Vector3[Div];
                Vector3[] bp = new Vector3[Div];
                for (int i = 0; i < Div; i++)
                {
                    float ratio = (float) i / Div * 360f * Mathf.Deg2Rad;
                    float x = BottomR * Mathf.Cos(ratio);
                    float z = BottomR * Mathf.Sin(ratio);
                    float tx = TopR * Mathf.Cos(ratio);
                    float tz = TopR * Mathf.Sin(ratio);
                    tp[i] = new Vector3(tx, Height, tz) + transform.position;
                    bp[i] = new Vector3(x, 0, z) + transform.position;
                }

                for (int p = Div - 1, q = 0; q < Div; p = q++)
                {
                    Debug.DrawLine(bp[p], bp[q], Color.red);
                    Debug.DrawLine(tp[p], tp[q], Color.blue);
                }

                for (int i = 0; i < Div; i++)
                {
                    //    bline[i] = (point[i + 1] + point[i]) / 2;
                    //    line[i] = (topPoint[i + 1] + topPoint[i]) / 2;
                    //    Debug.DrawLine(line[i], bline[i], Color.cyan, 100);
                    Debug.DrawLine(bp[i], tp[i], Color.yellow);
                }
            }
#endif
        }

        private void OnDestroy()
        {
//            print("OnDestroy");
            foreach (var col in bclist)
            {
                Destroy(col.gameObject);
            }
        }

        private void OnValidate()
        {
            if (Div < 1) Div = 1;
            float rR = Mathf.Abs(TopR - BottomR); //2つの半径の差を算出
            float rad = (Mathf.Atan2((TopR - BottomR), Height)); // /_ の角度 
            float hypo = Mathf.Sqrt((rR * rR + Height * Height)); //hypotenuse(斜辺)
            float unitr = 1f / Div * 360f; //一辺の有する角度

            Vector3[] center = Center();
            Vector3 size;
            if (attachObject != null)
                size = Vector3.one;
            else size =new Vector3(2 * Mathf.PI * TopR / Div, hypo, Width); //2PI
            
            Vector3 lr = Vector3.zero;
            lr.x = rad * Mathf.Rad2Deg;
            for (int i = 0; i < center.Length; i++)
            {
                if (bclist.Count > i && bclist[i] != null)
                {
                    lr.y = 90 - unitr * (i + 1) + unitr / 2;
                    bclist[i].transform.localEulerAngles = lr;
                    bclist[i].transform.localPosition = center[i];
                    bclist[i].material = CollidersMaterial;
                    bclist[i].size = size;
                    bclist[i].transform.SetParent(gameObject.transform, false);
                }
            }
        }

        public void GenerateCone()
        {
            Vector3[] center = Center();
            float unitr = 1f / Div * 360f;
            print("単位角度:" + unitr);
            float rR = Mathf.Abs(TopR - BottomR);
            float rad = (Mathf.Atan2(rR, Height));
            float half = unitr * Mathf.Deg2Rad / 2;
            float hypo = Mathf.Sqrt((rR * rR + Height * Height));

            Vector3 lr = Vector3.zero;
            lr.x = rad * Mathf.Rad2Deg;
            
            Vector3 size;
            if (attachObject != null)
                size = Vector3.one;
            else size =new Vector3(2 * Mathf.PI * TopR / Div, hypo, Width); //2PI
            for (int i = 0; i < Div; i++)
            {
                GameObject p = InstatiateCollider();
                SetName(i, p);
                lr.y = 90 - unitr * (i + 1) + unitr / 2;
                p.transform.localEulerAngles = lr;
                p.transform.localPosition = center[i];
                p.transform.SetParent(gameObject.transform, false);
                BoxCollider box = p.AddComponent<BoxCollider>();
                box.material = CollidersMaterial;
                box.size = size;
                bclist.Add(box);
            }
        }

        //Colliderのpositionを導出
        private Vector3[] Center()
        {
            Vector3[] center = new Vector3[Div];
            {
                Vector3[] point = new Vector3[Div];
                Vector3[] line = new Vector3[Div];
                Vector3[] bline = new Vector3[Div];
                Vector3[] topPoint = new Vector3[Div];
                for (int i = 0; i < Div; i++)
                {
                    float ratio = (float) i / Div * 360f * Mathf.Deg2Rad;
                    float sin = Mathf.Sin(ratio);
                    float cos = Mathf.Cos(ratio);
                    float x = BottomR * cos;
                    float z = BottomR * sin;
                    float tx = TopR * cos;
                    float tz = TopR * sin;
                    point[i] = new Vector3(x, 0, z);
                    topPoint[i] = new Vector3(tx, Height, tz);
//                if (i == 0) print(point[0]);
                }

                for (int i = 0; i < Div; i++)
                {
                    if (i + 1 < Div)
                    {
                        bline[i] = (point[i + 1] + point[i]) / 2;
                        line[i] = (topPoint[i + 1] + topPoint[i]) / 2;
                    }
                }

                bline[Div - 1] = (point[0] + point[Div - 1]) / 2;
                line[Div - 1] = (topPoint[0] + topPoint[Div - 1]) / 2;
                for (int i = 0; i < Div; i++)
                {
                    center[i] = new Vector3((bline[i].x + line[i].x) / 2,
                        Height / 2,
                        (bline[i].z + line[i].z) / 2);
                }
            }
            return center;
        }

        public void UpdateDivision()
        {
            foreach (BoxCollider bc in bclist)
            {
                if (bc != null)
                    DestroyImmediate(bc.gameObject);
            }

            foreach (var bc in GetComponentsInChildren<BoxCollider>())
            {
                if (bc != null)
                    DestroyImmediate(bc.gameObject);
                
            }

            bclist.Clear();
            GameObject ins = new GameObject();
            for (int i = 0; i < Div; i++)
            {
                GameObject p = InstatiateCollider();
                SetName(i, p);
                BoxCollider box = p.AddComponent<BoxCollider>();
                bclist.Add(box);
            }

            DestroyImmediate(ins);
            OnValidate();
        }

        private void SetName(int i, GameObject g)
        {
            g.name = colliderName + i;
        }

        private GameObject InstatiateCollider()
        {
            if (attachObject != null)
            {
                return Instantiate(attachObject);
            }
            else
            {
                return new GameObject();
            }
        }
    }
}