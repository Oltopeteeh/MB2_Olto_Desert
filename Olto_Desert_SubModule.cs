using System;
using System.IO;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Olto_Desert
{
    public class Olto_Desert_SubModule : MBSubModuleBase
	{
        public static float desert_num;
        public static float desert_mult;
        public static float lost_mult;

        protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
            this.LoadSettings();
        }
		private void LoadSettings()
		{
			Settings settings = new XmlSerializer(typeof(Settings)).Deserialize((Stream)File.OpenRead(Path.Combine(BasePath.Name, "Modules/Olto_Desert/settings.xml"))) as Settings;
            Olto_Desert_SubModule.desert_mult = settings.desert_mult;
            Olto_Desert_SubModule.desert_num = settings.desert_num;
            Olto_Desert_SubModule.lost_mult = settings.lost_mult;
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            gameStarterObject.AddModel(new Olto_Test_Model());
            gameStarterObject.AddModel(new OltoPartyDesertionModel());
        }
    }

    [Serializable]
    public class Settings
    {
        public float desert_mult;
        public float desert_num;
        public float lost_mult;
    }

    public class Olto_Test_Model : DefaultSmithingModel
    {
        public override int GetEnergyCostForRefining(ref Crafting.RefiningFormula refineFormula, Hero hero) { return 0; }
        public override int GetEnergyCostForSmithing(ItemObject item, Hero hero) { return 0; }
        public override int GetEnergyCostForSmelting(ItemObject item, Hero hero) { return 0; }
    }

    public class OltoPartyDesertionModel : DefaultPartyDesertionModel
    {
        public override int GetMoraleThresholdForTroopDesertion(MobileParty party)
        {
            return 10;
        }

        public override int GetNumberOfDeserters(MobileParty mobileParty)
        {
            bool num = mobileParty.IsWageLimitExceeded();
            bool flag = mobileParty.Party.NumberOfAllMembers > Olto_Desert_SubModule.desert_num + Olto_Desert_SubModule.desert_mult * mobileParty.LimitedPartySize;
            int result = 0;
            if (num)
            {
                int num2 = mobileParty.TotalWage - mobileParty.PaymentLimit;
                result = MathF.Min(20, MathF.Max(1, (int)((float)num2 / Campaign.Current.AverageWage * Olto_Desert_SubModule.lost_mult)));
            }
            else if (flag)
            {
                result = ((!mobileParty.IsGarrison) ? ((mobileParty.Party.NumberOfAllMembers > Olto_Desert_SubModule.desert_num + Olto_Desert_SubModule.desert_mult * mobileParty.LimitedPartySize) ? MathF.Max(1, (int)((float)(mobileParty.Party.NumberOfAllMembers - mobileParty.LimitedPartySize) * Olto_Desert_SubModule.lost_mult)) : 0) : MathF.Ceiling((float)(mobileParty.Party.NumberOfAllMembers - mobileParty.LimitedPartySize) * Olto_Desert_SubModule.lost_mult));
            }

            return result;
        }
    }
}