using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Moon.HabboHotel.Rooms.Chat.Logs;
using Moon.HabboHotel.Rooms.Chat.Filter;
using Moon.HabboHotel.Rooms.Chat.Emotions;
using Moon.HabboHotel.Rooms.Chat.Commands;
using Moon.HabboHotel.Rooms.Chat.Pets.Commands;
using Moon.HabboHotel.Rooms.Chat.Pets.Locale;
using log4net;
using Moon.HabboHotel.Rooms.Chat.Styles;

namespace Moon.HabboHotel.Rooms.Chat
{
    public sealed class ChatManager
    {
        private static readonly ILog log = LogManager.GetLogger("Moon.HabboHotel.Rooms.Chat.ChatManager");

        /// <summary>
        /// Chat Emoticons.
        /// </summary>
        private ChatEmotionsManager _emotions;

        /// <summary>
        /// Chatlog Manager
        /// </summary>
        private ChatlogManager _logs;

        /// <summary>
        /// Filter Manager.
        /// </summary>
        private WordFilterManager _filter;

        /// <summary>
        /// Commands.
        /// </summary>
        private CommandManager _commands;

        /// <summary>
        /// Pet Commands.
        /// </summary>
        private PetCommandManager _petCommands;

        /// <summary>
        /// Pet Locale.
        /// </summary>
        private PetLocale _petLocale;

        /// <summary>
        /// Chat styles.
        /// </summary>
        private ChatStyleManager _chatStyles;

        /// <summary>
        /// Initializes a new instance of the ChatManager class.
        /// </summary>
        public ChatManager()
        {
            this._emotions = new ChatEmotionsManager();
            this._logs = new ChatlogManager();
         
            this._filter = new WordFilterManager();
            this._filter.InitWords();
            this._filter.InitCharacters();

            this._commands = new CommandManager(":");
            this._petCommands = new PetCommandManager();
            this._petLocale = new PetLocale();
      
            this._chatStyles = new ChatStyleManager();
            this._chatStyles.Init();

            log.Info(">> Chat Manager -> Pronto!");
        }

        public ChatEmotionsManager GetEmotions()
        {
            return this._emotions;
        }

        public ChatlogManager GetLogs()
        {
            return this._logs;
        }

        public WordFilterManager GetFilter()
        {
            return this._filter;
        }

       public CommandManager GetCommands()
        {
            return this._commands;
        }

        public PetCommandManager GetPetCommands()
        {
            return this._petCommands;
        }

        public PetLocale GetPetLocale()
        {
            return this._petLocale;
        }

        public ChatStyleManager GetChatStyles()
        {
            return this._chatStyles;
        }
    }
}
