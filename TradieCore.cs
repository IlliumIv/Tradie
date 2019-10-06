using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using ExileCore.Shared.Enums;
using SharpDX;
using Vector2N = System.Numerics.Vector2;

namespace Tradie
{
    public class TradieCore : BaseSettingsPlugin<TradieSettings>
    {
        private readonly List<string> _whiteListedPaths = new List<string>
        {
                "Art/2DItems/Currency",
                "Art/2DItems/Maps"
        };

        public override bool Initialise()
        {
            Name = "Tradie";

            var WindowRect = GameController.Window.GetWindowRectangle();
            Settings.YourItemStartingLocationY.Max = WindowRect.Width;
            Settings.YourItemStartingLocationY.Max = WindowRect.Height;
            Settings.TheirItemStartingLocationX.Max = WindowRect.Width;
            Settings.TheirItemStartingLocationY.Max = WindowRect.Height;

            var combine = Path.Combine(DirectoryFullName, "background.png").Replace('\\', '/');
            Graphics.InitImage(combine, false);

            return true;
        }

        //public override void InitialiseMenu(MenuItem mainMenu)
        //{
        //    base.InitialiseMenu(mainMenu);
        //    var rootMenu = PluginSettingsRootMenu;
        //    MenuWrapper.AddMenu(rootMenu, "Image Size", Settings.ImageSize);
        //    MenuWrapper.AddMenu(rootMenu, "Text Size", Settings.TextSize);
        //    MenuWrapper.AddMenu(rootMenu, "Spacing", Settings.Spacing, "Spacing between image and text");
        //    var yourItems = MenuWrapper.AddMenu(rootMenu, "Your Trade Items");
        //    MenuWrapper.AddMenu(yourItems, "Ascending Order", Settings.YourItemsAscending);
        //    MenuWrapper.AddMenu(yourItems, "Currency Before Or After", Settings.YourItemsImageLeftOrRight, "On: <Currency> x<Amount>\nOff: <Amount>x <Currency>");
        //    MenuWrapper.AddMenu(yourItems, "Text Color", Settings.YourItemTextColor);
        //    MenuWrapper.AddMenu(yourItems, "Background Color", Settings.YourItemBackgroundColor);
        //    var yourItemLocation = MenuWrapper.AddMenu(yourItems, "Starting Location");
        //    MenuWrapper.AddMenu(yourItemLocation, "X", Settings.YourItemStartingLocationX);
        //    MenuWrapper.AddMenu(yourItemLocation, "Y", Settings.YourItemStartingLocationY);
        //    var theirItems = MenuWrapper.AddMenu(rootMenu, "Their Trade Items");
        //    MenuWrapper.AddMenu(theirItems, "Ascending Order", Settings.TheirItemsAscending);
        //    MenuWrapper.AddMenu(theirItems, "Currency Before Or After", Settings.TheirItemsImageLeftOrRight, "On: <Currency> x<Amount>\nOff: <Amount>x <Currency>");
        //    MenuWrapper.AddMenu(theirItems, "Text Color", Settings.TheirItemTextColor);
        //    MenuWrapper.AddMenu(theirItems, "Background Color", Settings.TheirItemBackgroundColor);
        //    var theirItemLocation = MenuWrapper.AddMenu(theirItems, "Starting Location");
        //    MenuWrapper.AddMenu(theirItemLocation, "X", Settings.TheirItemStartingLocationX);
        //    MenuWrapper.AddMenu(theirItemLocation, "Y", Settings.TheirItemStartingLocationY);
        //}

        public override void Render()
        {
            var Area = GameController.Game.IngameState.Data.CurrentArea;
            if (!Area.IsTown && !Area.RawName.Contains("Hideout")) return;
            ShowNpcTradeItems();
            ShowPlayerTradeItems();
        }

        private void ShowNpcTradeItems()
        {
            var npcTradingWindow = GetNpcTadeWindow();
            if (npcTradingWindow == null || !npcTradingWindow.IsVisible)
                return;
            var tradingItems = GetItemsInTradingWindow(npcTradingWindow);
            var ourData = new ItemDisplay
            {
                    Items = GetItemObjects(tradingItems.ourItems).OrderBy(item => item.Path),
                    X = Settings.YourItemStartingLocationX,
                    Y = Settings.YourItemStartingLocationY,
                    TextSize = Settings.TextSize,
                    TextColor = Settings.YourItemTextColor,
                    // BackgroundColor = Settings.YourItemBackgroundColor,
                    BackgroundColor = new Color(0, 0, 0),
                    BackgroundTransparency = Settings.YourItemBackgroundTransparency.Value,
                    ImageSize = Settings.ImageSize,
                    Spacing = Settings.Spacing,
                    LeftAlignment = Settings.YourItemsImageLeftOrRight,
                    Ascending = Settings.YourItemsAscending
            };
            var theirData = new ItemDisplay
            {
                    Items = GetItemObjects(tradingItems.theirItems).OrderBy(item => item.Path),
                    X = Settings.TheirItemStartingLocationX,
                    Y = Settings.TheirItemStartingLocationY,
                    TextSize = Settings.TextSize,
                    TextColor = Settings.TheirItemTextColor,
                    // BackgroundColor = Settings.TheirItemBackgroundColor,
                    BackgroundColor = new Color(0, 0, 0),
                    BackgroundTransparency = Settings.TheirItemBackgroundTransparency.Value,
                    ImageSize = Settings.ImageSize,
                    Spacing = Settings.Spacing,
                    LeftAlignment = Settings.TheirItemsImageLeftOrRight,
                    Ascending = Settings.TheirItemsAscending
            };
            if (ourData.Items.Any())
                DrawCurrency(ourData);
            if (theirData.Items.Any())
                DrawCurrency(theirData);
        }

        private void ShowPlayerTradeItems()
        {
            var tradingWindow = GetTradingWindow();
            if (tradingWindow == null || !tradingWindow.IsVisible)
                return;
            var tradingItems = GetItemsInTradingWindow(tradingWindow);
            var ourData = new ItemDisplay
            {
                    Items = GetItemObjects(tradingItems.ourItems).OrderBy(item => item.Path),
                    X = Settings.YourItemStartingLocationX,
                    Y = Settings.YourItemStartingLocationY,
                    TextSize = Settings.TextSize,
                    TextColor = Settings.YourItemTextColor,
                    // BackgroundColor = Settings.YourItemBackgroundColor,
                    BackgroundColor = new Color(0, 0, 0),
                    BackgroundTransparency = Settings.YourItemBackgroundTransparency.Value,
                    ImageSize = Settings.ImageSize,
                    Spacing = Settings.Spacing,
                    LeftAlignment = Settings.YourItemsImageLeftOrRight,
                    Ascending = Settings.YourItemsAscending
            };
            var theirData = new ItemDisplay
            {
                    Items = GetItemObjects(tradingItems.theirItems).OrderBy(item => item.Path),
                    X = Settings.TheirItemStartingLocationX,
                    Y = Settings.TheirItemStartingLocationY,
                    TextSize = Settings.TextSize,
                    TextColor = Settings.TheirItemTextColor,
                    // BackgroundColor = Settings.TheirItemBackgroundColor,
                    BackgroundColor = new Color(0, 0, 0),
                    BackgroundTransparency = Settings.TheirItemBackgroundTransparency.Value,
                    ImageSize = Settings.ImageSize,
                    Spacing = Settings.Spacing,
                    LeftAlignment = Settings.TheirItemsImageLeftOrRight,
                    Ascending = Settings.TheirItemsAscending
            };
            if (ourData.Items.Any())
                DrawCurrency(ourData);
            if (theirData.Items.Any())
                DrawCurrency(theirData);
        }

        private void DrawCurrency(ItemDisplay data)
        {
            const string symbol = "-";
            var counter = 0;
            // var newColor = data.BackgroundColor;
            var newColor = new Color(255, 255, 255, 0);
            newColor.A = (byte) data.BackgroundTransparency;
            var maxCount = data.Items.Max(i => i.Amount);

            string toMeasure = symbol + " " + maxCount;
            Vector2N measuredString = Graphics.MeasureText(toMeasure, data.TextSize);

            // Holy shit!
            var background = new RectangleF(
                data.LeftAlignment ? data.X : data.X + data.ImageSize,
                data.Y, 
                data.LeftAlignment ? + data.ImageSize + data.Spacing + 9
                    + measuredString.X : - data.ImageSize - data.Spacing - 9 - measuredString.X,
                data.Ascending ? - data.ImageSize * data.Items.Count() : data.ImageSize * data.Items.Count()
                );

            // Graphics.DrawBox(background, newColor);
            Graphics.DrawFrame(background, Color.DimGray, 1);

            foreach (var ourItem in data.Items)
            {
                counter++;
                var imageBox = new RectangleF(data.X,
                                              data.Ascending ? data.Y - counter * data.ImageSize : data.Y - data.ImageSize + counter * data.ImageSize,
                                              data.ImageSize,
                                              data.ImageSize);
                var textPlace = new Vector2(data.LeftAlignment ? data.X + data.ImageSize + data.Spacing : data.X - data.Spacing,
                                            imageBox.Center.Y - data.TextSize / 2 - 3);
                Graphics.DrawText(data.LeftAlignment ? $"{symbol} {ourItem.Amount}" : $"{ourItem.Amount} {symbol}",
                                  textPlace,
                                  data.TextColor,
                                  data.TextSize,
                                  data.LeftAlignment ? FontAlign.Left : FontAlign.Right);

                Graphics.DrawImage("background.png", new RectangleF(background.X, imageBox.Y, background.Width, imageBox.Height), newColor);
                DrawImage(ourItem.Path, imageBox);
            }
        }

        private bool DrawImage(string path, RectangleF rec)
        {
            try
            {
                Graphics.DrawImage(Path.GetFileName(path), rec);
            }
            catch (Exception e)
            {
                if (Settings.Debug)
                    LogError($"{Name}: Cannot DrawImage:\n" + e.Message, 10);
                return false;
            }

            return true;
        }

        private Element GetTradingWindow()
        {
            try
            {
                return GameController.Game.IngameState.UIRoot.Children[1].Children[79].Children[3].Children[1].Children[0].Children[0];
            }
            catch (Exception e)
            {
                if (Settings.Debug)
                    LogError($"{Name}: Cannot get TradingWindow:\n" + e.Message, 10);
                return null;
            }
        }

        private Element GetNpcTadeWindow()
        {
            try
            {
                return GameController.Game.IngameState.UIRoot.Children[1].Children[78].Children[3];
            }
            catch(Exception e)
            {
                if (Settings.Debug)
                    LogError($"{Name}: Cannot get NpcTradeWindow:\n" + e.Message, 10);
                return null;
            }
        }

        private (List<NormalInventoryItem> ourItems, List<NormalInventoryItem> theirItems) GetItemsInTradingWindow(Element tradingWindow)
        {
            var ourItemsElement = tradingWindow.Children[0];
            var theirItemsElement = tradingWindow.Children[1];
            var ourItems = new List<NormalInventoryItem>();
            var theirItems = new List<NormalInventoryItem>();

            // We are skipping the first, since it's a Element ("Place items you want to trade here") that we don't need.
            // 
            // skipping the first item as its a strange object added after 3.3 Incursion
            foreach (var ourElement in ourItemsElement.Children.Skip(2))
            {
                var normalInventoryItem = ourElement.AsObject<NormalInventoryItem>();
                if (normalInventoryItem == null)
                {
                    LogMessage("Tradie: OurItem was null!", 5);
                    throw new Exception("Tradie: OurItem was null!");
                }

                ourItems.Add(normalInventoryItem);
            }

            // skipping the first item as its a strange object added after 3.3 Incursion
            foreach (var theirElement in theirItemsElement.Children.Skip(1))
            {
                var normalInventoryItem = theirElement.AsObject<NormalInventoryItem>();
                if (normalInventoryItem == null)
                {
                    LogMessage("Tradie: OurItem was null!", 5);
                    throw new Exception("Tradie: OurItem was null!");
                }

                theirItems.Add(normalInventoryItem);
            }

            return (ourItems, theirItems);
        }

        private bool IsWhitelisted(string metaData) => _whiteListedPaths.Any(metaData.Contains);

        private IEnumerable<Item> GetItemObjects(IEnumerable<NormalInventoryItem> normalInventoryItems)
        {
            var items = new List<Item>();
            foreach (var normalInventoryItem in normalInventoryItems)
                try
                {
                    if (normalInventoryItem.Item == null) continue;
                    string metaData = "";
                    // if(normalInventoryItem.Item.HasComponent<RenderItem>())
                        metaData = normalInventoryItem.Item.GetComponent<RenderItem>().ResourcePath;
                    if (metaData.Equals("")) continue;
                    var stack = normalInventoryItem.Item.GetComponent<Stack>();
                    var amount = stack?.Info == null ? 1 : stack.Size;
                    var name = GetImagePath(metaData, normalInventoryItem);
                    var found = false;
                    foreach (var item in items)
                        if (item.ItemName.Equals(name))
                        {
                            item.Amount += amount;
                            found = true;
                            break;
                        }

                    if (found) continue;
                    if (!IsWhitelisted(metaData))
                        continue;
                    var path = GetImagePath(metaData, normalInventoryItem);
                    items.Add(new Item(name, amount, path));
                }
                catch (Exception e)
                {
                    LogError($"{Name}: Sometime went wrong in GetItemObjects() for a brief moment\n{e.Message}\n{e.StackTrace}", 5);
                }

            return items;
        }

        private string GetImagePath(string metadata, NormalInventoryItem invItem)
        {
            metadata = metadata.Replace(".dds", ".png");
            var url = $"http://webcdn.pathofexile.com/image/{metadata}";
            var metadataPath = metadata.Replace('/', '\\');
            var fullPath = $"{DirectoryFullName}\\images\\{metadataPath}";

            /////////////////////////// Yucky Map bits ///////////////////////////////
            if (invItem.Item.HasComponent<Map>())
            {
                var isShapedMap = 0;
                var mapTier = invItem.Item.GetComponent<Map>().Tier;
                foreach (var itemList in invItem.Item.GetComponent<Mods>().ItemMods)
                {
                    if (!itemList.RawName.Contains("MapShaped")) continue;
                    isShapedMap = 1;
                    mapTier += 5;
                }

                url = $"http://webcdn.pathofexile.com/image/{metadata}?mn=1&mr={isShapedMap}&mt={mapTier}";
                fullPath = $"{DirectoryFullName}\\images\\{metadataPath.Replace(".png", "")}_{mapTier}_{isShapedMap}.png";
            }
            //

            if (File.Exists(fullPath))
            {
                Graphics.InitImage(fullPath, false);
                return fullPath;
            }
            var path = fullPath.Substring(0, fullPath.LastIndexOf('\\'));
            Directory.CreateDirectory(path);
            using (var client = new WebClient())
            {
                client.DownloadFile(new Uri(url), fullPath);
                Graphics.InitImage(fullPath, false);
            }

            return fullPath;
        }
    }
}