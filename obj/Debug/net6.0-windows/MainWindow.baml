   M S B A M L   `   `   `  ????I  Ecircle display, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:circle_displaycircle_display  5   	       circle_display.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:circle_display     5   	   -   5          ? Title$  
MainWindow??5   	   $	??800??6   $
??1200??6)    ? WindowState$ 	Maximized=?6M   .??5	      ?? $
??Auto=?6   .??5
   
   ??  ?? Orientation$ 
Horizontal=?6   .+?5      ?? $??Colours??6   $	??200??6(   $??Right=?64   $	??Top=?6P   .??5      ?? $ Vertical=?6   .+?5      ?? $ 
Horizontal=?6!   .+?5      ?? -   5        $??lblIntensity??5       $??60??64   $??25??6?   $??Center=?6K   $??5q?6m   .??6x   
Intensity:6?   ?? 5      -   5         ??Name$ sldIntensity??5   !    ? Maximum$	 255Q?67    ?? TickFrequency$ 1Q?6E    ?? IsSnapToTickEnabled$
 True??6W    ?? AutoToolTipPlacement$ TopLeft=?6r   $??0,10,0,0q?6?    ? Value$	 100Q?6?   ${?False??6?   $	??110??6?   6?   5      ?? 5      $ 
Horizontal=?6!   .+?5      ?? -   5        $??lblRedSlider??5       $??60??64   $??25??6?   $??Center=?6K   $??5q?6m   .??6x   Red:6~   ?? 5      -   5        $ sldRed??5   !   $	 255Q?61   $ 1Q?6?   $
 True??6Q   $ TopLeft=?6l   $??0,10,0,0q?6?   $	 255Q?6?   $	??110??6?   6?   5      ?? 5      $ 
Horizontal=?6!   .+?5      ?? -   5        $??lblGreenSlider??5       $??60??66   $??25??6A   $??Center=?6M   $??5q?6o   .??6z   Green:6?   ?? 5      -   5        $ sldGreen??5   !   $	 255Q?63   $ 1Q?6A   $
 True??6S   $ TopLeft=?6n   $??0,10,0,0q?6?   $	 255Q?6?   $	??110??6?   6?   5      ?? 5      $ 
Horizontal=?6!   .+?5      ?? -   5        $??lblBlueSlider??5       $??60??65   $??25??6@   $??Center=?6L   $??5q?6n   .??6y   Blue:6?   ?? 5      -	   5        $ sldBlue??5   !   $	 255Q?62   $ 1Q?6@   $
 True??6R   $ TopLeft=?6m   $??0,10,0,0q?6?   $	 255Q?6?   $	??110??6?   6?   5      ?? 5      $??Preset Colours??6   .??5      ?? $ 
Horizontal=?6%   $??Center=?6>   .+?5      ?? -
   5        	 ??Name$	 btnRedPreset??5   %   $??40??6;   $??30??6F   $??5q?6R   .??6x   Red6}   ?? 5       -   5        $	 btnWhitePreset??5    %   $??40??6=   $??30??6H   $??5q?6T   .??6|   White6?   5!      5"      ?? 5#      -   5        $	 	btnExport??5#      $??80??60   $??30??6;   $??5q?6G   $??Center=?6R   $	??Top=?6o   .??6?   Export6?   ?? 5$      -   5        $	 	btnImport??5$      $??80??60   $??30??6;   $??5q?6G   $??Center=?6R   $	??Top=?6o   .??6?   Import6?   ?? 5%      -   5        $	 btnReset??5%      $??80??6/   $??30??6:   $??	0,25,0,10q?6F   $??Center=?6Y   $	??Top=?6v   .??6?   Reset6?   5&      5'      ?? 5(      -   5        
 ??Name$
 cnvsCircles??5(      $
??1300??6*   $
??Left=?68   $	??Top=?6S   $??10q?6k   6y   5)      5*      5+      