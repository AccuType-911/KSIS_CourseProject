   M S B A M L   `   `   `  ����B  >Torrent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:TorrentTorrent  5   	       Torrent.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:Torrent     5   	   -   5          �Name$  MainWindow1��5   	    � Title$ Торрент��5   	   $	��450��6   $	��960��6&    � 
ResizeMode$ NoResize=�62    � Icon$ icon.ico��6H    � 	Resources 5
         AddButtonImage �� 	UriSource  ResumeDownloadButtonImage  StopDownloadButtonImage��%�   &	        &	 9     &	 }     �� 5   
   $% ../resources/AddButtonImage.png?�6-   �� 5   
   $0 *../resources/ResumeDownloadButtonImage.png?�68   �� 5   
   $. (../resources/StopDownloadButtonImage.png?�66   5      .��5      � -   5         �Name$ WindowsGrid��5      .+�5   
   o� $��25��6   $��AcmeFont)�6   $	��Top=�61   ��5      �� 5       �� EndPoint$	 0,1&�6&   .��5      �  � Color$ 	#FFF6F6F6��6#   	 � Offset$
	 0.25Q�65   � 5      $ 	#FFEAE8E8��6#   $
	 0.25Q�65   � 5      $ 	#FFDCD9D9��6#   $		 0.8Q�65   � 5      $ White��6#   $	 1Q�61   5      .@�5      m� -   5        $��SettingsMenuItem��5      $��Настройки��6/   6c   m� 5      $��Справка��6   6+   5      &   System.Windows.Controls.DataGrid  5   
   -   5        
  Name$
 TorrentsDataGrid��5         
IsReadOnly$
 True��6E      AutoGenerateColumns$ False��6W   $��
0,69,0,157q�6s   $	��880��6�   $��Center=�6�   ��6-   ��    Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$	 №��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 35 6K   1  +System.Windows.Controls.DataGridBoundColumn   Binding 60   �� *Number+6K     5       $ Состояние��6%   $	 150 6R    68   �� *State+6R     5!      $ Название��6%   $	 150 6P    67   �� *Name+6P     5"      $ Прогресс��6%   $	 150 6U    67   �� *
Progress+6U     5#      $  Скорость загр.��6%   $	 100 6_    6=   �� *DownloadSpeed+6_     5$      $ Скорость отд.��6%   $	 100 6\    6<   �� *UploadSpeed+6\     5%      $ Загружено��6%   $	 100 6[    68   �� *DownloadedData+6[     5&      $ Отдано��6%   $	 100 6V    65   �� *UploadedData+6V   5(      �� 5)   
   -   5         ��Name$ ButtonsStackPanel��5)      $��	0,368,0,0q�60    �� Orientation$ 
Horizontal=�6C   ���	#7FB8B8B8$��66��6s   .+�5*      �� -   5         ��Name$ AddTorrentButton��5*      $	��Top=�6/   
���   �
���   � J� OpacityMask �	#FC000000$��32��6�   $	��305��6�     FirstButtonStyle#��[ 6�   .��5+      �� $ 
Horizontal=�6   
�������$��16(�6U   .+�5,      �� -   5         ��Name$ AddTorrentButtonImage��5,      #��[  6;   �� 5-      $��Center=�6    $'��! Добавить торрент ��6=   $��Center=�6W   5.      5/      �� 50      -   5        $ StopDownloadButton��50      $	��Top=�61   
���   �
���   �$��32��6p   $	��305��6|     SecondButtonStyle#��[ 6�   .��51      �� $ 
Horizontal=�6   
�������$��16(�6U   .+�52      �� -	   5        $ StopDownloadButtonImage��52      #��[ 6=   �� 53      $��Center=�6    $-��' Остановить загрузку ��6=   $��Center=�6Z   54      55      �� 56      -
   5        $ ResumeDownloadButton��56      $	��Top=�63   
���   �
���   �$��32��6r   $	��305��6~     ThirdButtonStyle#��[ 6�   .��57      �� $ 
Horizontal=�6   
�������$��16(�6U   .+�58      �� -   5        $ ResumeDownloadButtonImage��58      #��[ 6?   �� 59      $��Center=�6    $/��) Возобновить загрузку ��6=   $��Center=�6[   5:      5;      5<      �� 5=   
   $&�� Список торрентов:��6   $��Center=�6,   $��0,30,0,0q�6I   $	��Top=�6[   $	��880��6s   $��18(�6   ���	#7FB8B8B8$
��Bold"�6�   �� 5>   
   -   5         ��Name$ CommonInfoTextBox��5>      $
��Left=�6-    �� 
IsReadOnly$
 True��6H   $��212,262,0,0q�6Z    �� TextWrapping$
 Wrap=�6o   $	��Top=�6�   $	��708��6�   $	��101��6�   ���	#7FB8B8B86�   �� 5?   
   -   5        $ DeleteButton��5?      $��40,262,753,0q�6'   $	��Top=�6=   
���   �
���   �$��30��6|     DeleteButtonStyle#��[ 6�   ���	#BF4B4B4B.��5@      �� $ 
Horizontal=�6   
�������$��16(�6Q   .+�5A      �� $��Center=�6   $%�� Удалить торрент ��69   $��Center=�6R   5B      5C      5D      5E      