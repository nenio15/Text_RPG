using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EventInformer : MonoBehaviour
{
    // 여기서 이벤트들의 여하여부를 결정
    // 확률도 얻고 그럼. ㅇㅇ
    // nearby를 textchanger에게서 얻어낸다.
    // 그거를 반영은. 매니저한테?
    // 다른 스크립트한테 알려주는 건 좋아. 근데 얻어내는건 이 스크립트가 직접 해내야지.

    // Start is called before the first frame update
    [SerializeField] private Textchanger textchanger;


    // 필요없는 함수. jmp에서 얻는 걸로 전부 해결할거야.
    public void getNear() // 이 스크립트가 제때 알려줘? 아님, 매니저가 필요할때 불러?
    {
        foreach(JToken jnear in textchanger.jbase["nearby"]) // nearby 전부 토해내!!!!
        {
            Debug.Log(jnear.ToString());

        }

    }
}
