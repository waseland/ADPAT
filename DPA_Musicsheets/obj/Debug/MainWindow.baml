   M S B A M L   `   `   `  ����P  LPSAMWPFControlLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\Bclr-namespace:PSAMWPFControlLibrary;assembly=PSAMWPFControlLibraryPSAMWPFControlLibrary  5   	   J FDPA_Musicsheets, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DPA_MusicsheetsDPA_Musicsheets 5   	       DPA_Musicsheets.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ]PSAMWPFControlBclr-namespace:PSAMWPFControlLibrary;assembly=PSAMWPFControlLibrary     5   	   /localclr-namespace:DPA_Musicsheets     5   	   -   5          � Title$  
MainWindow��5	   	   $��846.5��6   $��1531.5��6+   .��5      � -   5         �Name$ MainGrid��5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $	��500 �6   �� 5      $	��500 �6   �� 5      $	��500 �6   .+�5   
   � $��0��6    5      �� 5      $��60 �6    �� 5      $
��Auto �6     5      �� 5      $
��Auto �6#   .+�5!      �� -   5         ��Name$ txt_MidiFilePath��5!   #   $��0��6   $
��Left=�6=   $��23��6X   $��	90,10,0,0q�6d    �� TextWrapping$
 Wrap=�6w   $4��.../../../Alle-eendjes-zwemmen-in-het-water.mid��6�   $	��Top=�6�   $	��233��6�   �� 5"      -   5         ��Name$ btnPlay��5"      $
��Play��6&   $
��Left=�65   $��
335,40,0,0q�6P   $	��Top=�6d   $��75��6|   6�   �� 5#      -   5        $ btnOpen��5#   "   $��0��6   $
��Open��63   $
��Left=�6B   $��	10,11,0,0q�6]   $	��Top=�6p   $��75��6�   6�   �� 5$      -   5        $ btn_Stop��5$      $
��Stop��6'   $
��Left=�66   $��
415,40,0,0q�6Q   $	��Top=�6e   $��75��6}   6�   �� 5%      -   5        $ btn_ShowContent��5%   "   $��0��6   $��Show content��6;   $
��Left=�6R   $��
335,11,0,0q�6m   $	��Top=�6�   $	��155��6�   6�   �� 5'      -   5         ��Name$ tabCtrl_MidiContent��5'   &   $��1��6   $
��Left=�6C   $	��700��6^   $��10,0,0,0q�6k   $	��Top=�6}   $	��480��6�   ��6�   �� ��5(      �� 5)       �� 
TargetType" ��6   .�5*      �� 	 �� Property	 � ��
 �� Value
 5+      �� 5,   "    �� Path$ 	TrackName�6*   5.      5/       �� ContentTemplate 51      �� 52      .R�53      �� ��6"   �� *
Messages+��54      �� 55   "   .R�56   &   �� ��6,   �� *.+57   #   59      5:      5<      5=      � 5?   
   $��1��6   .+�5@      �� -	   5         ��Name$ barlinesScrollViewer��5@      $��Disabled=�69   ��. $	��800��6{   $��	0,10,0,10q�6�   .��5A      �� -
   5         ��Name$ barlinesStackPanel��5A      $��5,0,5,0q�69   .+�5B      ,   &PSAMWPFControlLibrary.IncipitViewerWPF  -   5          Name$ staff��5B   ]   $
��Left=�66   $	��486��6Q   $	��231��6l   5C      5D      5E      5F      � 5G   
   $��2��6   .+�5H      �� -   5        $ btnUndo��5H   4   $��0��6   {�.  $
��Undo��6E   $
��Left=�6T   $��	20,11,0,0q�6o   $	��Top=�6�   $��75��6�   6�   �� 5I      -   5        $ btnRedo��5I   4   $��0��6   {�.  $
��Redo��6E   $
��Left=�6T   $��
100,11,0,0q�6o   $	��Top=�6�   $��74��6�   6�   �� 5J      -   5        $ 	btnUpdate��5J   4   $��0��6   {�.  $��Update to Sheet��6G   $
��Left=�6a   $��
179,11,0,0q�6|   $	��Top=�6�   $��99��6�   6�   �� 5K      -   5        $ lilypondText��5K      $	��760��6,    �� AcceptsReturn . $��10,50,-10,10q�6N   .��6d   5M      5N      5O      5P      