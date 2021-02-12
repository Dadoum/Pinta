// 
// Styles.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Gtk;
using Hexadock;
using MonoDevelop.Components;
using Xwt.Drawing;

namespace Pinta.Docking
{
	public static class Styles
	{
		public static readonly Cairo.Color BaseBackgroundColor = new Cairo.Color (1, 1, 1);
		public static readonly Cairo.Color BaseForegroundColor = new Cairo.Color (0, 0, 0);

		private static StyleContext StyleContext
		{
			get
			{
				using (Label box = new Label())
				{
					var styleContext = box.StyleContext;
					// styleContext.AddProvider(CssProvider.Default, 0);
					return styleContext;
				}
			}
		}
		
		
		// General

		public static Gdk.RGBA ThinSplitterColor
		{
			get
			{
				Gdk.RGBA color;
				using (var sep = new Separator(Orientation.Vertical))
				{
					color = sep.StyleContext.GetBackgroundColor(StateFlags.Normal);
					color.Alpha *= 1.75;
				}
				return color;
			}
		}
			/*new Gdk.RGBA
		{
			Red = 166 / 256, 
			Green = 166 / 256, 
			Blue = 166 / 256,
			Alpha = 1
		}; // */
		
		// Document tab bar


		public static Cairo.Color TabBarBackgroundColor => ThinSplitterColor.ToCairoColor();

		public static Cairo.Color TabBarActiveTextColor
		{
			get
			{
				StyleContext.LookupColor("text_color", out var rgba);
				rgba.Alpha = 1;
				return rgba.ToCairoColor();
			}
		}

		public static Cairo.Color TabBarActiveGradientStartColor => Shift (TabBarBackgroundColor, 0.92);
		public static  Cairo.Color TabBarActiveGradientEndColor => TabBarBackgroundColor;
		public static Cairo.Color TabBarGradientStartColor => Shift (TabBarBackgroundColor, 1.02);
		public static Cairo.Color TabBarGradientEndColor => TabBarBackgroundColor;
		public static Cairo.Color TabBarGradientShadowColor => Shift (TabBarBackgroundColor, 0.8);
		public static Cairo.Color TabBarHoverActiveTextColor => TabBarActiveTextColor;
		public static Cairo.Color TabBarInactiveTextColor => Blend (TabBarHoverInactiveTextColor, TabBarGradientStartColor, 0.4);
		public static readonly Cairo.Color TabBarHoverInactiveTextColor = new Cairo.Color (0, 0, 0);

		public static Cairo.Color BreadcrumbBackgroundColor => BreadcrumbGradientStartColor;

		public static Cairo.Color BreadcrumbGradientStartColor
		{
			get
			{
				try
				{

					StyleContext.AddClass("background");
					var val_rgba = StyleContext.LookupColor("bg_color", out var rgba);
					StyleContext.RemoveClass("background");
					return rgba.ToCairoColor();
				}
				catch (Exception e)
				{
					Console.WriteLine("haha {0}", e);
					throw e;
				}
			}
		}
		public static Cairo.Color BreadcrumbGradientEndColor => BreadcrumbGradientStartColor;// Shift (BreadcrumbGradientStartColor, 0.9);

		public static Cairo.Color BreadcrumbInactiveGradientStartColor
		{
			get
			{
				var hsl = BreadcrumbGradientStartColor.ToXwtColor();
				hsl.Light *= 0.9;
				return hsl.ToCairoColor();
			}
		}// Shift (BreadcrumbGradientStartColor, .95);
		public static Cairo.Color BreadcrumbInactiveGradientEndColor => BreadcrumbInactiveGradientStartColor;// Shift (BreadcrumbGradientStartColor, 0.9);
		public static Cairo.Color BreadcrumbBorderColor => Shift (TabBarActiveGradientStartColor, 0.6);
		public static Cairo.Color BreadcrumbInnerBorderColor => WithAlpha (BaseBackgroundColor, 0.1d);
		public static Gdk.RGBA BreadcrumbTextColor => Shift (BaseForegroundColor, 0.8).ToXwtColor().ToRGBA();
		public static Cairo.Color BreadcrumbButtonBorderColor => Shift (BaseBackgroundColor, 0.8);
		public static Cairo.Color BreadcrumbButtonFillColor => WithAlpha (BaseBackgroundColor, 0.1d);
		public static Cairo.Color BreadcrumbBottomBorderColor => Shift (BreadcrumbBackgroundColor, 0.7d);
		public static readonly bool BreadcrumbInvertedIcons = false;
		public static readonly bool BreadcrumbGreyscaleIcons = false;

		// Dock pads
		
		public static readonly Cairo.Color DockTabBarGradientTop = new Cairo.Color (248d / 255d, 248d / 255d, 248d / 255d);
		public static readonly Cairo.Color DockTabBarGradientStart = new Cairo.Color (242d / 255d, 242d / 255d, 242d / 255d);
		public static readonly Cairo.Color DockTabBarGradientEnd = new Cairo.Color (230d / 255d, 230d / 255d, 230d / 255d);
		public static readonly Cairo.Color DockTabBarShadowGradientStart = new Cairo.Color (154d / 255d, 154d / 255d, 154d / 255d, 1);
		public static readonly Cairo.Color DockTabBarShadowGradientEnd = new Cairo.Color (154d / 255d, 154d / 255d, 154d / 255d, 0);

		public static Gdk.RGBA PadBackground
		{
			get
			{
				StyleContext.LookupColor("bg_color", out var color);
				return color;
			}
		}

		public static Gdk.RGBA InactivePadBackground => TabBarActiveGradientStartColor.ToXwtColor().ToRGBA();
		public static readonly Gdk.RGBA PadLabelColor = new Gdk.RGBA
		{
			Red = 92 / 256, 
			Green = 99 / 256, 
			Blue = 102 / 256,
			Alpha = 0.5
		};
		public static readonly Gdk.RGBA DockFrameBackground = new Gdk.RGBA
		{
			Red = 157 / 256, 
			Green = 162 / 256, 
			Blue = 166 / 256,
			Alpha = 0.5
		};
		public static Gdk.RGBA DockSeparatorColor => ThinSplitterColor;

		public static readonly Gdk.RGBA BrowserPadBackground = new Gdk.RGBA
		{
			Red = 225 / 256, 
			Green = 228 / 256, 
			Blue = 232 / 256,
			Alpha = 0.5
		};

		public static readonly Gdk.RGBA InactiveBrowserPadBackground = new Gdk.RGBA
		{
			Red = 240 / 256, 
			Green = 240 / 256, 
			Blue = 240 / 256,
			Alpha = 0.5
		};

		public static readonly Cairo.Color DockBarBackground1 = PadBackground.ToCairoColor ();
		public static readonly Cairo.Color DockBarBackground2 = PadBackground.ToCairoColor(); // Shift (PadBackground.ToCairoColor (), 0.95);
		public static readonly Cairo.Color DockBarSeparatorColorDark = new Cairo.Color (0, 0, 0, 0.2);

		public static Cairo.Color DockBarSeparatorColorLight => DockSeparatorColor.ToCairoColor(); // = new Cairo.Color (1, 1, 1, 0.3);

		public static Cairo.Color DockBarPrelightColor
		{
			get
			{
				var hsl = BreadcrumbBackgroundColor.ToXwtColor();
				hsl.Light *= 1.2;
				return hsl.ToCairoColor();
			}
		}

		// Status area

		public static readonly Cairo.Color WidgetBorderColor = CairoExtensions.ParseColor ("8c8c8c");

		public static readonly Cairo.Color StatusBarBorderColor = CairoExtensions.ParseColor ("919191");

		public static readonly Cairo.Color StatusBarFill1Color = CairoExtensions.ParseColor ("f5fafc");
		public static readonly Cairo.Color StatusBarFill2Color = CairoExtensions.ParseColor ("e9f1f3");
		public static readonly Cairo.Color StatusBarFill3Color = CairoExtensions.ParseColor ("d8e7ea");
		public static readonly Cairo.Color StatusBarFill4Color = CairoExtensions.ParseColor ("d1e3e7");

		public static readonly Cairo.Color StatusBarErrorColor = CairoExtensions.ParseColor ("FF6363");

		public static readonly Cairo.Color StatusBarInnerColor = new Cairo.Color (0,0,0, 0.08);
		public static readonly Cairo.Color StatusBarShadowColor1 = new Cairo.Color (0,0,0, 0.06);
		public static readonly Cairo.Color StatusBarShadowColor2 = new Cairo.Color (0,0,0, 0.02);
		public static readonly Cairo.Color StatusBarTextColor = CairoExtensions.ParseColor ("555555");
		public static readonly Cairo.Color StatusBarProgressBackgroundColor = new Cairo.Color (0, 0, 0, 0.1);
		public static readonly Cairo.Color StatusBarProgressOutlineColor = new Cairo.Color (0, 0, 0, 0.1);

		public static readonly Pango.FontDescription StatusFont = Pango.FontDescription.FromString ("Normal");

		public static int StatusFontPixelHeight { get { return (int)(11 * PixelScale); } }
		public static int ProgressBarHeight { get { return (int)(18 * PixelScale); } }
		public static int ProgressBarInnerPadding { get { return (int)(4 * PixelScale); } }
		public static int ProgressBarOuterPadding { get { return (int)(4 * PixelScale); } }

		private static readonly double PixelScale = 1;// Gdk.Screen.Default.GetMonitorScaleFactor();

		// Toolbar

		public static readonly Cairo.Color ToolbarBottomBorderColor = new Cairo.Color (0.5, 0.5, 0.5);
		public static readonly Cairo.Color ToolbarBottomGlowColor = new Cairo.Color (1, 1, 1, 0.2);

		// Code Completion

		public static readonly int TooltipInfoSpacing = 1;

		// Popover Windows

		public static class PopoverWindow
		{
			public static readonly int PagerTriangleSize = 6;
			public static readonly int PagerHeight = 16;

			public static readonly Cairo.Color DefaultBackgroundColor = CairoExtensions.ParseColor ("fff3cf");
			public static readonly Cairo.Color ErrorBackgroundColor = CairoExtensions.ParseColor ("E27267");
			public static readonly Cairo.Color WarningBackgroundColor = CairoExtensions.ParseColor ("efd46c");
			public static readonly Cairo.Color InformationBackgroundColor = CairoExtensions.ParseColor ("709DC9");

			public static readonly Cairo.Color DefaultBorderColor = CairoExtensions.ParseColor ("ffeeba");
			public static readonly Cairo.Color ErrorBorderColor = CairoExtensions.ParseColor ("c97968");
			public static readonly Cairo.Color WarningBorderColor = CairoExtensions.ParseColor ("e8c12c");
			public static readonly Cairo.Color InformationBorderColor = CairoExtensions.ParseColor ("6688bc");

			public static readonly Cairo.Color DefaultTextColor = CairoExtensions.ParseColor ("665a36");
			public static readonly Cairo.Color ErrorTextColor = CairoExtensions.ParseColor ("ffffff");
			public static readonly Cairo.Color WarningTextColor = CairoExtensions.ParseColor ("563b00");
			public static readonly Cairo.Color InformationTextColor = CairoExtensions.ParseColor ("ffffff");

			public static class ParamaterWindows
			{
				public static readonly Cairo.Color GradientStartColor = CairoExtensions.ParseColor ("fffee6");
				public static readonly Cairo.Color GradientEndColor = CairoExtensions.ParseColor ("fffcd1");
			}
		}

		// Helper methods

		internal static Cairo.Color Shift (Cairo.Color color, double factor)
		{
			return new Cairo.Color (color.R * factor, color.G * factor, color.B * factor, color.A);
		}

		internal static Cairo.Color WithAlpha (Cairo.Color c, double alpha)
		{
			return new Cairo.Color (c.R, c.G, c.B, alpha);
		}

		internal static Cairo.Color Blend (Cairo.Color color, Cairo.Color targetColor, double factor)
		{
			return new Cairo.Color (color.R + ((targetColor.R - color.R) * factor),
			                        color.G + ((targetColor.G - color.G) * factor),
			                        color.B + ((targetColor.B - color.B) * factor),
			                        color.A
			                        );
		}

		internal static Cairo.Color MidColor (double factor)
		{
			return Blend (BaseBackgroundColor, BaseForegroundColor, factor);
		}

		internal static Cairo.Color ReduceLight (Cairo.Color color, double factor)
		{
			var c = color.ToXwtColor ();
			c.Light *= factor;
			return c.ToCairoColor ();
		}

		internal static Cairo.Color IncreaseLight (Cairo.Color color, double factor)
		{
			var c = color.ToXwtColor ();
			c.Light += (1 - c.Light) * factor;
			return c.ToCairoColor ();
		}

		internal static Gdk.RGBA ReduceLight (Gdk.RGBA color, double factor)
		{
			return ReduceLight (color.ToCairoColor (), factor).ToXwtColor().ToRGBA();
		}

		internal static Gdk.RGBA IncreaseLight (Gdk.RGBA color, double factor)
		{
			return IncreaseLight (color.ToCairoColor (), factor).ToXwtColor().ToRGBA();
		}
	}
}

