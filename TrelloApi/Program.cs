using Manatee.Trello;
using Manatee.Trello.Exceptions;
using Manatee.Trello.ManateeJson;
using Manatee.Trello.RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrelloApi
{
    class Program
    {
        const string lastPublish = "- Última publicação:";
        const string templateFind = "- **{0}:**";
        const string endString = "****";

        private static string cardID = "CARD_ID";
        private static string titleSearch = "TITLE_SEARCH";

        private const string appKey = "APP_KEY";
        private const string userToken = "USER_TOKEN";

        /// <summary>
        /// 
        ///  1* Dev Key
        ///  https://trello.com/app-key
        ///  
        ///  2* Approve app Name = TrelloForDB1
        ///  https://trello.com/1/connect?key=DEV_KEY_GENERATE_FIRST_PROCESS&name=TrelloForDB1&response_type=token&scope=read,write
        ///  
        ///  3* TESTE
        ///  https://trello.com/1/members/my/cards?key=DEV_KEY_GENERATE_FIRST_PROCESS&token=TOKEN_GENERATE_SECOND_PROCESS
        /// 
        /// </summary>
        /// <param name="args">cardId, TitleToModified</param>
        static void Main(string[] args)
        {
            if ((args.Length == 1) && ((args[0].ToUpper().Contains("/H")) ||
                                            (args[0].ToUpper().Contains("-H"))))
            {
                HelpSystem();
                return;
            }
            else if (args.Length != 2)
            {
                System.Console.WriteLine("Argument not specified or invalid. See /help");
                Console.ReadKey();
                return;
            }

            cardID = args[0];
            titleSearch = args[1];

            var serializer = new ManateeSerializer();
            TrelloConfiguration.Serializer = serializer;
            TrelloConfiguration.Deserializer = serializer;
            TrelloConfiguration.JsonFactory = new ManateeFactory();
            TrelloConfiguration.RestClientProvider = new RestSharpClientProvider();
            TrelloAuthorization.Default.AppKey = appKey;
            TrelloAuthorization.Default.UserToken = userToken;

            try
            {
                NewDescriptionBuild();

                Console.WriteLine("Bing!!! Success!!!");
            }
            catch (TrelloInteractionException trelloExcpetion)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Trello :(");
                Console.WriteLine("");
                Console.WriteLine(trelloExcpetion.InnerException.Message);
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("BUG: Oh ohhh!!");
                Console.WriteLine("");
                Console.WriteLine(e.Message);
            }
            
            Console.ReadKey();
        }

        private static void NewDescriptionBuild()
        {
            var card = new Card(cardID);

            var indexTitle = card.Description.IndexOf(string.Format(templateFind, titleSearch));
            var descriptionCuted = card.Description.Substring(indexTitle);

            var index = descriptionCuted.IndexOf(string.Format(templateFind, titleSearch));
            var indexEnd = descriptionCuted.IndexOf(endString);
            var totalToCut = (indexEnd + endString.Count()) - index;

            var descriptionFiltered = descriptionCuted.Substring(index, totalToCut);


            var indexEndLastPublish = descriptionFiltered.IndexOf(endString);
            var indexLastPublish = descriptionFiltered.IndexOf(lastPublish);
            var cutLastPublish = indexEndLastPublish - indexLastPublish;

            var publishFrom = descriptionFiltered.Substring(indexLastPublish, cutLastPublish);
            var publishTo = string.Format("{0} {1}\n\n", lastPublish, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            var newDescription = card.Description.Replace(publishFrom, publishTo);

            card.Description = newDescription;
        }

        private static void HelpSystem()
        {
            System.Console.WriteLine("first parameter: card id");
            Console.BackgroundColor = ConsoleColor.Blue;
            System.Console.WriteLine("sample: https://trello.com/c/nloBsKZg/6-versionamento - card id =  nloBsKZg");
            Console.ResetColor();
            System.Console.WriteLine(" ");
            System.Console.WriteLine("second parameter: title to search");
            Console.BackgroundColor = ConsoleColor.Blue;
            System.Console.WriteLine("sample: PRO or HOM");
            Console.ResetColor();
            System.Console.WriteLine(" - **PRO:**");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("   - Último Script Banco (AAM): 30/01/2015 12:00");
            System.Console.WriteLine("   - Último Script Banco (Controle Acesso): 30/01/2015 12:00");
            System.Console.WriteLine("   - Última publicação: 30/01/2015 16:00");
            System.Console.WriteLine(" ");
            System.Console.WriteLine(" ****");
            System.Console.WriteLine(" ");
            System.Console.WriteLine(" - **HOM:**");
            System.Console.WriteLine(" ");
            System.Console.WriteLine("   - Último Script Banco (AAM): 03/02/2015 - 11:00");
            System.Console.WriteLine("   - Último Script Banco (Controle Acesso): 14/11/2014 11:15");
            System.Console.WriteLine("   - Última publicação: 22/02/2015 19:50");
            System.Console.WriteLine(" ");
            System.Console.WriteLine(" ****");
            System.Console.WriteLine(" ");
            Console.ReadKey();
        }
    }
}
