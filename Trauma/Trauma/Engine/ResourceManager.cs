using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using TiledLib;

namespace Trauma.Engine
{
    /// <summary>
    /// Manages all the textures of the game.
    /// Can be used for texture storage and retrieval.
    /// </summary>
    public static class ResourceManager
    {
        const char NAME_SEPARATOR = '_';
        const char DIR_SEPARATOR = '/';

        #region Textures 
        /* main texture directory name */
        const string TEXTURE_DIR_NAME = "Textures";
        /* texture subdirectory names */
        const string PLAYER_DIR_NAME = "Player";
        const string PORTAL_DIR_NAME = "Portal";
        const string BLOB_DIR_NAME = "Blob";
        const string GENERATOR_DIR_NAME = "Generator";
        const string SPLATTER_DIR_NAME = "Splatter";
        const string BACKGROUND_DIR_NAME = "Background";
        const string MISC_DIR_NAME = "Misc";

        static Dictionary<String, Texture2D> texDic = new Dictionary<string,Texture2D>();
        
        /* All the textures, divided by category. */

        private static readonly List<String> playerTexNames = new List<String>
            {
                "Main",
                "Main_Splatter",
                "Icon"
            };
        static readonly List<String> portalTexNames = new List<String>
            {
                "Spin1",
                "Spin2",
                "Spin3",
                "Spin4",
                "Symbol1_Anger",
                "Symbol2_Anger",
                "Symbol3_Anger",
                "Symbol4_Anger",
                "Symbol1_Depression",
                "Symbol2_Depression",
                "Symbol4_Depression",
                "Icon"
            };

        private static readonly List<String> splatterTexNames = new List<String>
            {
                "1",
                "2",
                "3",
                "4"
            };
        static readonly List<String> blobTexNames = new List<String>
            {
                "Main",
                "Main_Black",
                "Main_Water"
            };
        private static readonly List<String> generatorTexNames = new List<string>
            {
                "Icon",
                "Drip",
                "Drip_Black",
                "Drip_Water",
                "Wave",
                "Wave_Water",
                "Wave_Black",
                "Spout",
                "Spout_Water",
                "Spout_Black"
            }; 
        static readonly List<String> backgroundTexNames = new List<string>
            {
                "Intro",
                "Denial",
                "Depression",
                "Anger"
            }; 
        static readonly List<String> miscTexNames = new List<String>
            {
                "Pixel",
                "UndoIcon",
                "Navigation",
                "Toolbar",
                "Spikes",
                "Barrier"
            };

        /* Maps each path to a list of textures. */
        static readonly Dictionary<String, List<String>> texPathDic = new Dictionary<string, List<string>>
        {
            { PLAYER_DIR_NAME, playerTexNames },
            { PORTAL_DIR_NAME, portalTexNames },
            { SPLATTER_DIR_NAME, splatterTexNames },
            { BLOB_DIR_NAME, blobTexNames },
            { GENERATOR_DIR_NAME, generatorTexNames },
            { BACKGROUND_DIR_NAME, backgroundTexNames },
            { MISC_DIR_NAME, miscTexNames }
        };

        /// <summary>
        /// Load all textures.
        /// </summary>
        /// <param name="Content">The content manager of the game.</param>
        public static void LoadTextures(ContentManager Content)
        {
            foreach (String path in texPathDic.Keys)
                foreach (String name in texPathDic[path])
                    texDic[path + NAME_SEPARATOR + name] =
                        Content.Load<Texture2D>(TEXTURE_DIR_NAME + DIR_SEPARATOR + path + DIR_SEPARATOR + name);
        }

        /// <summary>
        /// Gets the texture specified by the given name.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <returns>The texture specified by the name.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the name is not a valid texture name.</exception>
        public static Texture2D GetTexture(String name)
        {
            return texDic[name];
        }

        /// <summary>
        /// Get a random splatter image.
        /// </summary>
        /// <returns>A splatter texture.</returns>
        public static Texture2D GetRandomSplatter()
        {
            Random random = new Random();
            int splatterIndex = random.Next(splatterTexNames.Count);
            return GetTexture(SPLATTER_DIR_NAME + NAME_SEPARATOR + splatterTexNames[splatterIndex]);
        }

        #endregion

        #region Fonts
        const string FONT_DIR_NAME = "Fonts";

        static List<String> fontNames = new List<String> {
            "Credits"
        };

        static Dictionary<String, SpriteFont> fontDic = new Dictionary<string,SpriteFont>();

        public static void LoadFonts(ContentManager content) 
        {
            foreach (String fontName in fontNames) 
            {
                fontDic[fontName] = content.Load<SpriteFont>(FONT_DIR_NAME + DIR_SEPARATOR + fontName);
            }        
        }

        public static SpriteFont GetFont(String name) 
        {
            return fontDic[name];
        }

        #endregion
        #region Sounds
        /* Main sound directory. */
        const string SOUND_DIR_NAME = "Sounds";

        /* All sound sub-directories. */
        const string MUSIC_DIR_NAME = "Music";
        const string EFFECT_DIR_NAME = "Effect";

        static Dictionary<String, Song> soundDic = new Dictionary<string, Song>();

        static readonly List<String> musicNames = new List<String>
        {
            "Anger",
            "Acceptance",
            "Bargain",
            "Depression",
            "Denial", 
        };

        static readonly List<String> effectNames = new List<String>();

        static readonly Dictionary<String, List<String>> soundPathDic = new Dictionary<string, List<string>> 
        {
            { MUSIC_DIR_NAME, musicNames },
            { EFFECT_DIR_NAME, effectNames }
        };

        public static void LoadSounds(ContentManager Content)
        {
            foreach (String path in soundPathDic.Keys)
                foreach (String name in soundPathDic[path])
                    soundDic[name] =
                        Content.Load<Song>(SOUND_DIR_NAME + DIR_SEPARATOR + path + DIR_SEPARATOR + name);
        }

        /// <summary>
        /// Gets the sound specified by the given name.
        /// </summary>
        /// <param name="name">The name of the sound.</param>
        /// <returns>The sound specified by the name.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the name is not a valid sound name.</exception>
        public static Song GetSound(String name)
        {
            return soundDic[name];
        }

        public static void PlaySong(String name)
        {
            MediaPlayer.Play(soundDic[name]);
        }

        public static void Stop()
        {
            MediaPlayer.Stop();
        }
        #endregion

        #region Maps

        /* Main maps directory. */
        const string MAPS_DIR_NAME = "Maps";

        /* All map subdirectories. */
        const string INTRO_DIR_NAME = "Intro";
        const string DENIAL_DIR_NAME = "Denial";
        const string ANGER_DIR_NAME = "Anger";
        const string BARGAIN_DIR_NAME = "Bargain";
        const string DEPRESSION_DIR_NAME = "Depression";
        const string ACCEPTANCE_DIR_NAME = "Acceptance";

        // it would be really wasteful to have all the maps loaded into memory
        // at once, so instead only allow for individual maps to be requested.

        /// <summary>
        /// Get the map specified by the given name.
        /// </summary>
        /// <param name="name">The name of the map.</param>
        /// <param name="Content">The content manager of the game.</param>
        /// <returns>Return the map specified by the given name.</returns>
        public static Map LoadMap(String name, ContentManager Content)
        {
            string[] parts = name.Split(NAME_SEPARATOR);
            String path = String.Join(DIR_SEPARATOR.ToString(), parts);
            return Content.Load<Map>(MAPS_DIR_NAME + DIR_SEPARATOR + path);
        }
        #endregion
    }
}
