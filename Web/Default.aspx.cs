using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SteamParty.Core;

namespace SteamParty.Web
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var steamids = TextBox1.Text.Split(',');

            var api = new SteamApi(ConfigurationManager.AppSettings["ApiKey"]);

            var comparer = new Comparer(api);

            var games = comparer.Compare(Array.ConvertAll(steamids, long.Parse));

            var sb = new StringBuilder();
            sb.Append("<table><tr><th>Game</th><th>Hours</th><th>Players</th></tr>");

            foreach (var game in games)
            {
                sb.Append(string.Format("<tr><td>{0}<br/><img src='{1}'/></td><td>{2}</td><td>", game.Key.Name, game.Key.LogoUrl, game.Key.Playtime));
                foreach (var player in game.Value)
                {
                    sb.Append(string.Format("<img src='{0}' />", player.AvatarUrl64));
                }
                sb.Append("</td></tr>");
            }

            sb.Append("</table>");
            Literal1.Text = sb.ToString();
        }


    }
}