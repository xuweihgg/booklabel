   M S B A M L   `   `   `  ����P  LBookLabel.LabelModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:BookLabel.LabelModule.ViewsBookLabel.LabelModule.Views  5      K*clr-namespace:BookLabel.LabelModule.ModelsBookLabel.LabelModule.Models  5      O,clr-namespace:BookLabel.LabelModule.ControlsBookLabel.LabelModule.Controls  5      =#clr-namespace:BookLabel.LabelModuleBookLabel.LabelModule  5	      *    $BookLabel.LabelModule.Views.MuenView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:BookLabel.LabelModule.Views     5      <model*clr-namespace:BookLabel.LabelModule.Models     5      @control,clr-namespace:BookLabel.LabelModule.Controls     5      5event#clr-namespace:BookLabel.LabelModule     5	        � 	Resources  5       	  FIcon �� 
TargetType �� Property �� Value  	MenuFIcon  ItemsContentBackground  TextForeground  ItemMouseOverBackground  ItemMouseOverForeground  ItemSelectedBackground  ItemSelectedForeground  VerticalContentAlignment 	 DefaultTreeViewItem �� 
TargetType ��Name �� Padding � UseLayoutRounding J� SnapsToDevicePixels	 � ColumnDefinitions
 d�Name �� 	ClickMode d� 	IsChecked �� Path �� TemplatedParent �� RelativeSource �� Mode ��Name ��Name ��Name �� Triggers P� Property P� Value �� 
TargetName ]� 
Conditions �� Property �� Value 
 DefaultTreeView K� IsItemsHost %� IsVirtualizing %� VirtualizationMode  ItemNode �� DataType6   0BookLabel.LabelModule.Models.CatalogConstruction �� ItemsSource  �� Orientation4   .BookLabel.LabelModule.Controls.EditableTextBox!   
IsEditable"   Text#   IsInEditMode��%�  &	        &	     &	     &	 U    &	 �    &	 �    &	 ;    &	 �    &	 �    &		 (    &	
 j    7 	 &	 *    �� 5   
   " ��6   .�5      ��  � ��$/ )pack://application:,,,/Resource/#iconfont)�6+   6_   �� 5       �@��TextAlignment$ Center=�6.   �� 5       � ��$ Center=�64   �� 5       � ��$ Center=�62   �� 5       � ��$ 20(�6)   5      �� 5   
   " m�6"   .�5      ��  �@m�Icon$/ )pack://application:,,,/Resource/#iconfont��6%   6Y   �� 5       � ��$ Center=�64   �� 5       � ��$ Center=�62   �� 5       � ��$ 20(�6)   5      �� 5   
   " Q�6/   .�5      ��  � ��
 �����6?   5      �� 5   
   " Q�6'   .�5      ��  � ��
 �����6;   5      �� 5    
   " Q�60   .�5!      ��  � �� �#F20BA06=   5"      �� 5#   
   " Q�60   .�5$      ��  � �� �#FB0AE86=   5%      �� 5'   
   " Q�6/   .�5(      ��  � �� �#12B5126=   5)      �� 5+   
   " Q�6/   .�5,      ��  � �� �#FEDB116=   5-      �� 5/   
   " Q�61   .�50      ��  � ��$ Center=�62   6C   51      �� 53   
   " Q�6-   .�54      ��  � ��$ 20��6'   �� 55       � ��
 �����6+   �� 56       � �� �#A145F86+   �� 58       �@J�SnapsToDevicePixels . 64   �� 59       � ��$ 2q�6'   �� 5:       � �� 5;      �� 5<      " Q�6&   .R�5=      �� .+�5>      �� $ Bd��6%   #��z��61   #��z��6[   #��z��5?   !   # z��6U   #��z��5@   !    . # z 6b   ._�5C   "   � #��z��6'   $��Stretch=�6I   	 5D   &   �� 5E   *   $��20��6;   $
��Auto �6I   �� 5F   *   $��* �6;   .+�5I   &   d� $
 ExpanderBtn��63   $ Press=�5K   /    5J   /   �� $ 
IsExpanded�# Z $ TwoWay=���5L   *   �� 5M   .   " d�6>   .R�5N   2   �� ._�5O   6   �� $��Center=�6G   $��Center=�6b   # z 6   5P   3   5Q   /   ��5S   *   �� 5T   .   $ ExpanderIcon��68   $	����6O   #���   6_   5V   '   �� 5X   &   $ PART_Header��67   $��1��6L   $��Header��6\   # z 5Y   3   #��z��5Z   3   #��z��5[   3   5\   #   5]      �� 5^      $ 	ItemsHost��6?   $��18,0,0,0q�6-   # z 6R   5_       5`      P� 5a       �@Q�
IsExpanded .  . �5b   "   �� $ 	ItemsHost��6)    � y�$ 	Collapsed=�6V   5c      P� 5d       �@Q�
IsExpanded . . �5e   "   �� $ ExpanderIcon��6)    � ��$	 ��6S   5f      P� 5g       �@��HasItems .  . �5h   "   �� $ ExpanderIcon��6)    � y�$ Hidden=�6Y   5i      P� 5j       �@J�IsMouseOver . . �5k   "   ��  � �� �#F080006?   5l      P� 5m       �@Q�
IsSelected . . �5n   "   ��  � �� �#F0A0006?   5o      ]� 5p       5q   "   �� 5r   &    �@Q�
IsSelected . 6F   �� 5s   &    �@��IsSelectionActive . 6V   ./�5u   "   ��  � �� �#F0A0006?   5v      5x      5z      5{      �� 5~   
   " R�6(   .�5      ��  � �� . 6>   �� 5�       �@%�IsVirtualizing . 6U   �� 5�       �@%�VirtualizationMode$ 	Recycling=�6J   �� 5�      " �@��IsDeferredScrollingEnabled .  6H   �� 5�       � �� �#007AC06+   �� 5�       � ��8   6b   �� 5�       � �� 5�      �� 5�      .R�5�      $�  .  . $ 	Recycling=�6Z   $��0q�6y   5�      5�      5�      �� 5�   
   "  65    6c   �� *ChirdCatalogs+.R�5�      �� $  
Horizontal=�6   .+�5�      �� $	����6   #���   6,     5�      ! . $��12(�5�   *   $	��100��68   $��Center=�6G   " 5�   *   �� *CatalogName+# 6Y   �� *IsInEditMode+$ TwoWay=�5�   g   5�      5�      5�      .��5�      � $ � RowDefinitions$ 5�   
   �� 5�      $��50 �6   �� 5�      $��* �6   .+�5�   
   �� $��新建目录��6   $��24��6I   ���#A145F8
���������6    �� *InsertCatalogCommand+6j   R� 5�   
   -   5        % R�Name$% tree1��5�   8   $��1��6   $	��250��6    $��3q�6-   #��[ 6H   
������ ��6q   �� *
Catalogs+��5�      �� 5�      & �� BasedOn#& [	 6   " Q�6G   5�      ' � ContextMenu' 5�      �� 5�      .@�5�      m� ( m� Icon$	( ��6    $��增加��61   ) m� Command) 6=   �� *InsertChirdCommand+m� 5�      $	( ��6    $��修改��60   ) 6<   �� *UpdateCatalogCommand+m� 5�      $	( ��6    $��删除��60   ) 6<   �� *DeleteCatalogCommand+5�      5�      5�      5�      