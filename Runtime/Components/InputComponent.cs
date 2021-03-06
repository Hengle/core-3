using Facebook.Yoga;
using Jint.Native;
using Jint.Native.Function;
using ReactUnity.Styling;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ReactUnity.Components
{
    public class InputComponent : ContainerComponent
    {
        public static YogaNode InputDefaultLayout { get; } = new YogaNode()
        {
            Padding = 8,
            MinHeight = 40,
            MinWidth = 200,
            MaxWidth = YogaValue.Percent(100),
            Overflow = YogaOverflow.Hidden
        };

        public static NodeStyle InputDefaultStyle { get; } = new NodeStyle()
        {
            backgroundColor = Color.white,
            borderRadius = 8,
            fontSize = 24,
            cursor = "text",
        };

        public override YogaNode DefaultLayout => InputDefaultLayout;
        public override NodeStyle DefaultStyle => InputDefaultStyle;

        public string Value
        {
            get => InputField.text;
            set
            {
                InputField.text = value;
                Placeholder.ApplyStyles();
            }
        }


        private TMP_InputField InputField { get; set; }
        private ContainerComponent TextViewport { get; set; }
        private TextComponent TextComponent { get; set; }
        private TextComponent Placeholder { get; set; }

        public InputComponent(string text, UnityUGUIContext context) : base(context, "input")
        {
            // Input field's properties must be fully assigned before OnEnable is called
            GameObject.SetActive(false);

            Selectable = InputField = GameObject.AddComponent<TMP_InputField>();

            TextViewport = new ContainerComponent(context, "");
            TextViewport.GameObject.name = "[Text Viewport]";
            TextViewport.SetParent(this);
            TextViewport.GameObject.AddComponent<RectMask2D>();


            Placeholder = new TextComponent("", context);
            Placeholder.GameObject.name = "[Placeholder]";
            Placeholder.Layout.PositionType = YogaPositionType.Absolute;
            Placeholder.SetParent(TextViewport);


            TextComponent = new TextComponent(text, context);
            TextComponent.GameObject.name = "[Text]";
            TextComponent.SetParent(TextViewport);
            TextComponent.Flex.enabled = false;
            var textRect = TextComponent.RectTransform;
            textRect.pivot = Vector2.one / 2;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;


            InputField.textViewport = TextViewport.RectTransform;
            InputField.textComponent = TextComponent.Text;
            InputField.placeholder = Placeholder.Text;
            InputField.fontAsset = TextComponent.Text.font;

            GameObject.SetActive(true);
            SetText(text);
        }

        public void SetText(string text)
        {
            InputField.text = text;
        }

        public override void ApplyLayoutStyles()
        {
            base.ApplyLayoutStyles();
            TextComponent.SelfControl.enabled = Layout.Width.Unit == YogaUnit.Auto;
        }

        public override void ResolveStyle(bool recursive = false)
        {
            base.ResolveStyle(recursive);

            var c = TextComponent.Style.fontColor;
            Placeholder.Style.fontColor = new Color(c.r, c.g, c.b, c.a * 0.5f);
            Placeholder.ApplyStyles();
        }

        public override void ApplyStyles()
        {
            base.ApplyStyles();
            InputField.pointSize = Style.fontSizeActual;
        }

        public void Focus()
        {
            InputField.Select();
        }


        public override void SetEventListener(string eventName, FunctionInstance callback)
        {
            switch (eventName)
            {
                case "onEndEdit":
                    InputField.onEndEdit.RemoveAllListeners();
                    if (callback != null) InputField.onEndEdit.AddListener(new UnityAction<string>(x => callback.Invoke(x)));
                    return;
                case "onSubmit":
                    InputField.onSubmit.RemoveAllListeners();
                    if (callback != null) InputField.onSubmit.AddListener(new UnityAction<string>(x => callback.Invoke(x)));
                    return;
                case "onChange":
                    InputField.onValueChanged.RemoveAllListeners();
                    if (callback != null) InputField.onValueChanged.AddListener(new UnityAction<string>(x => callback.Invoke(x)));
                    return;
                case "onTextSelection":
                    InputField.onTextSelection.RemoveAllListeners();
                    if (callback != null) InputField.onTextSelection.AddListener(
                        new UnityAction<string, int, int>((x, i, j) => callback.Invoke(x, i, j)));
                    return;
                case "onEndTextSelection":
                    InputField.onEndTextSelection.RemoveAllListeners();
                    if (callback != null) InputField.onEndTextSelection.AddListener(
                        new UnityAction<string, int, int>((x, i, j) => callback.Invoke(x, i, j)));
                    return;
                default:
                    base.SetEventListener(eventName, callback);
                    return;
            }
        }

        public override void SetProperty(string propertyName, object value)
        {
            switch (propertyName)
            {
                case "placeholder":
                    Placeholder.SetText(value.ToString());
                    return;
                case "value":
                    InputField.text = value.ToString();
                    return;
                case "characterLimit":
                    InputField.characterLimit = System.Convert.ToInt32(value);
                    return;
                case "lineLimit":
                    InputField.lineLimit = System.Convert.ToInt32(value);
                    return;
                case "readonly":
                    InputField.readOnly = System.Convert.ToBoolean(value);
                    return;
                case "richText":
                    InputField.richText = System.Convert.ToBoolean(value);
                    return;
                case "contentType":
                    InputField.contentType = (TMP_InputField.ContentType)System.Convert.ToInt32(value);
                    return;
                case "keyboardType":
                    InputField.keyboardType = (TouchScreenKeyboardType)System.Convert.ToInt32(value);
                    return;
                case "lineType":
                    InputField.lineType = (TMP_InputField.LineType)System.Convert.ToInt32(value);
                    return;
                case "validation":
                    InputField.characterValidation = (TMP_InputField.CharacterValidation)System.Convert.ToInt32(value);
                    return;
                case "webSupport":
                    var enabled = System.Convert.ToBoolean(value);
                    var cmp = SetWebGLSupport(enabled);
                    if (!enabled && cmp) Object.Destroy(cmp);
                    return;
                default:
                    base.SetProperty(propertyName, value);
                    break;
            }
        }

        private WebSupport.WebGLInput SetWebGLSupport(bool createIfDoesNotExist)
        {
            var cmp = GameObject.GetComponent<WebSupport.WebGLInput>();
            if (createIfDoesNotExist && !cmp) GameObject.AddComponent<WebSupport.WebGLInput>();
            return cmp;
        }
    }
}
