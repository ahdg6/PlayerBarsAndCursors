using ReLogic.Content.Sources;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace PlayerBarsAndCursors {
	public class PlayerBarsAndCursors : Mod {

		public static PlayerBarsAndCursors Instance {  get; set; }
		public PlayerBarsAndCursors() => Instance = this;

        public override IContentSource CreateDefaultContentSource() {
			return base.CreateDefaultContentSource();
		}
    }
}