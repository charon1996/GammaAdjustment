   M S B A M L   `   `   `  ����J  FGammaAdjustment, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:GammaAdjustmentGammaAdjustment  5   	        GammaAdjustment.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:GammaAdjustment     5   	   -   5          � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)    J� 	AllowDrop$
 True��5	   	    � 	Resources 5      ��%    5      .��5      �  � ColumnDefinitions 5   
   �� 5      �� 5      �� 5      $	��180 �6   .+�5   
   � $��0��6   .+�5      �� -   5         ��Name$ SourceImageTxt��5      �� 5      -   5         ��Name$ SourceImageContainer��5      5      � 5   
   $��1��6   .+�5      �� -   5        $ ComparisonImageTxt��5      �� 5      -   5        $ AdjustedImageContainer��5      5       � 5#   
   $��2��6   .+�5$      R� $��Right=�6   $	��180��63   .@�5%      Q� $��
Brightness��6    Q� 
IsExpanded$
 True��63   .@�5&      Q� $��Gamma��6#   $
 True��62   .@�5'      ��  �� Orientation$ Vertical=�6%   .+�5(      �� $ 
Horizontal=�6)   $��
-40,0,0,10q�6C   .+�5)   "   �� $��R:��6,   �� 5*   "   -   5         ��Name$ slider_bightness_r��5*   )   	 � Minimum$	 0Q�6E   
 � Maximum$	
 100Q�6Q    � Value$ 0Q�6_   $	��120��6i    �� IsSnapToTickEnabled$
 True��6u   6�   �� 5+   "   ��6,   ��  �� Path$ Value� �� ElementName$ slider_bightness_r��5,      �� 5-      $ 
Horizontal=�6)   $��
-40,0,0,10q�6C   .+�5.   "   �� $��G:��6,   �� 5/   "   -   5        $ slider_bightness_g��5/   )   $	 0Q�6E   $	
 100Q�6Q   $ 0Q�6_   $	��120��6i   $
 True��6u   6�   �� 50   "   ��6,   �� $ Value�$ slider_bightness_g��51      �� 52      $ 
Horizontal=�6)   $��	-40,0,0,0q�6C   .+�53   "   �� $��B:��6,   �� 54   "   -   5        $ slider_bightness_b��54   )   $	 0Q�6E   $	
 100Q�6Q   $ 0Q�6_   $	��120��6i   $
 True��6u   6�   �� 55   "   ��6,   �� $ Value�$ slider_bightness_b��56      57      59      5:      5<      5>      5?      5@      