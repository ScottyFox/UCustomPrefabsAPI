using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
namespace UCustomPrefabsAPI.ContentWarning.CustomActions
{
    public class CW_VisorTextHelper : CustomActionsBase
    {
        private const float CW_Visor_MaxSize = 0.04f;
        private Player Player = null;
        private HashSet<TextMeshPro> TextMeshPro_Targets = new HashSet<TextMeshPro>();
        private string Text = string.Empty;
        private float Rotation = 0;
        private float FaceSize = 1.0f;
        public override void RegisterActions()
        {
            AddOnStateChanged(AttachTargets);
            AddOnUpdate(Update);
        }
        public void AttachTargets(string last, string current)
        {
            Player = Handler.GetComponent<Player>();
            if (!Player)
                return;
            foreach (var tagged in Handler.GetTagsInTemplates("UseVisorText"))
            {
                var target = tagged.GetComponent<TextMeshPro>();
                if (!target)
                    continue;
                FixTextOffsets(target);
                TextMeshPro_Targets.Add(target);
            }
            Update();
        }
        public void FixTextOffsets(TextMeshPro target)
        {
            if (target.rectTransform.localScale == Vector3.one && target.rectTransform.localRotation == Quaternion.identity)
                return;
            var newContainer = new GameObject($"{target.name}(Container)");
            newContainer.transform.SetParent(target.transform.parent);
            newContainer.transform.localPosition = target.transform.localPosition;
            newContainer.transform.localScale = target.transform.localScale;
            newContainer.transform.localRotation = target.transform.localRotation;
            target.transform.SetParent(newContainer.transform);
            target.transform.localPosition = Vector3.zero;
            target.transform.localScale = Vector3.one;
            target.transform.localRotation = Quaternion.identity;
        }
        public void Verify()
        {
            Debug.LogWarning("VisorText seems to be invalid, Verifying.");
            var Refreshed = new HashSet<TextMeshPro>();
            foreach (var target in TextMeshPro_Targets)
            {
                if (target)
                    Refreshed.Add(target);
                else
                    Debug.LogWarning("Removing TextMeshPro from VisorTextHelper!");
            }
            TextMeshPro_Targets = Refreshed;
        }
        public void Update()
        {
            if (!Player)
                return;
            var playerVisor = Player.refs.visor;
            if (
                    playerVisor.visorFaceText.transform.localEulerAngles.z == Rotation
                &&
                    playerVisor.FaceSize == FaceSize
                &&
                    playerVisor.visorFaceText.text == Text
                )
                return;
            Rotation = playerVisor.visorFaceText.transform.localEulerAngles.z;
            FaceSize = playerVisor.FaceSize;
            Text = playerVisor.visorFaceText.text;
            var newScale = Vector3.one * (FaceSize / CW_Visor_MaxSize);
            var newRotation = new Vector3(0, 0, Rotation);
            bool valid;
            do
            {
                valid = true;
                foreach (var target in TextMeshPro_Targets)
                {
                    if (!target)
                    {
                        valid = false;
                        break;
                    }
                    target.rectTransform.localEulerAngles = newRotation;
                    target.rectTransform.localScale = newScale;
                    target.text = Text;
                }
                if (!valid)
                    Verify();
            } while (!valid);
        }
    }
}