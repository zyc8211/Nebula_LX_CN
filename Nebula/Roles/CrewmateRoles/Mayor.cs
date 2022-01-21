﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hazel;
using UnityEngine;

namespace Nebula.Roles.CrewmateRoles
{
    public class Mayor : Role
    {
        static public Color Color = new Color(3f / 255f, 79f / 255f, 66f / 255f);

        public int votesId { get; private set; }

        private Module.CustomOption maxVoteOption;
        private Module.CustomOption maxVoteStockOption;

        //投じる票数の表示
        private TMPro.TextMeshPro countText;

        //今投票したときに投じる票数
        private byte numOfVote=1;

        public override void GlobalInitialize(PlayerControl __instance)
        {
            Game.GameData.data.players[__instance.PlayerId].SetRoleData(votesId, 0);
        }

        public override void OnMeetingStart()
        {
            PlayerControl.LocalPlayer.GetModData().AddRoleData(votesId, 1);
        }

        public override void OnVote(byte targetId)
        {
            RPCEventInvoker.MultipleVote(PlayerControl.LocalPlayer, numOfVote);
            RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId,votesId,-numOfVote);
        }

        public override void SetupMeetingButton(MeetingHud __instance)
        {
            numOfVote = 1;

            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                GameObject template, button;
                PassiveButton passiveButton;
                SpriteRenderer renderer;

                template = __instance.SkipVoteButton.Buttons.transform.Find("CancelButton").gameObject;
                button = UnityEngine.Object.Instantiate(template, __instance.SkipVoteButton.transform);
                button.name = "MayorButton";
                button.transform.position += new Vector3(1.5f, 0f);
                renderer = button.GetComponent<SpriteRenderer>();
                renderer.sprite = Images.GlobalImage.GetMeetingButtonLeft();
                passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick.RemoveAllListeners();
                passiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
                {
                    if (numOfVote > 0)
                        numOfVote--;
                }));

                template = __instance.SkipVoteButton.Buttons.transform.Find("CancelButton").gameObject;
                button = UnityEngine.Object.Instantiate(template, __instance.SkipVoteButton.transform);
                button.name = "MayorButton";
                button.transform.position += new Vector3(2.7f, 0f);
                renderer = button.GetComponent<SpriteRenderer>();
                renderer.sprite = Images.GlobalImage.GetMeetingButtonRight();
                passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick.RemoveAllListeners();
                passiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
                {
                    if (numOfVote < maxVoteOption.getFloat() && numOfVote < Game.GameData.data.myData.getGlobalData().GetRoleData(votesId))
                        numOfVote++;
                }));



                countText = UnityEngine.Object.Instantiate(__instance.TitleText, __instance.SkipVoteButton.transform);
                countText.alignment = TMPro.TextAlignmentOptions.Center;
                countText.transform.position = __instance.SkipVoteButton.CancelButton.transform.position;
                countText.transform.position += new Vector3(1.54f, 0f);
                countText.color = Palette.White;
                countText.transform.localScale *= 0.8f;
                countText.text = "";
            }
        }

        public override void MeetingUpdate(MeetingHud __instance, TMPro.TextMeshPro meetingInfo)
        {
            int count= Game.GameData.data.myData.getGlobalData().GetRoleData(votesId);

            meetingInfo.text = Language.Language.GetString("role.mayor.voteStockLeft") + ": " + count;
            meetingInfo.gameObject.SetActive(true);

            countText.text = numOfVote.ToString();
        }

        public override void LoadOptionData()
        {
            maxVoteOption = CreateOption(Color.white, "maxVote", 5f, 0f, 20f, 1f);
            maxVoteStockOption = CreateOption(Color.white, "maxVoteStock", 5f, 0f, 20f, 1f);
        }

        public Mayor()
            : base("Mayor", "mayor", Color, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
                 Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
                 false, false, false, false, false)
        {
            votesId = Game.GameData.RegisterRoleDataId("mayor.votes");
        }
    }
}
