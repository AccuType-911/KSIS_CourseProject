   M S B A M L   `   `   `  ����B  >Torrent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:TorrentTorrent  5   	   !    Torrent.DeleteTorrentWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:Torrent     5   	   -   5          �Name$  DeleteTorrentWindow1��5   	    � Title$' !Удаление торрента��5   	   $	��270��6#   $	��400��60    � Icon$ icon.ico��6<    � 
ResizeMode$ NoResize=�6L   .��5	      � .+�5
   
   �� -   5         ��Name$&  DeleteFromTorrentsFolderCheckBox��5
      $7��1Удалить файл из папки Torrents.��6=   $
��Left=�6g   $��	10,57,0,0q�6�   $	��Top=�6�   �� 5   
   -   5        $' !DeleteFromDownloadsFolderCheckBox��5      $[��UУдалить загруженное содержимое из папки Downloads.��6>   $
��Left=�6{   $��	10,78,0,0q�6�   $	��Top=�6�   �� 5   
   $(��"Удаление торрента.��6   $
��Left=�6-   $��	10,10,0,0q�6H   $	��Top=�6[   ���	#7FB8B8B8$��18(�6�   $
��Bold"�6�   �� 5   
   -   5         ��Name$ DeleteButton��5      $��10,186,216,0q�6'   $	��Top=�6=   
���   �
���   �$��30��6|      DeleteButtonStyle#��[  6�   ���	#BF4B4B4B.��5      ��  �� Orientation$ 
Horizontal=�6   
�������$��16(�6Q   .+�5      �� $��Center=�6   $�� Подтвердить ��69   $��Center=�6N   5      5      �� 5   
   -   5        $ DeleteButton_Copy��5      $��210,186,16,0q�6,   $	��Top=�6B   
���   �
���   �$��30��6�   #��[  6�   ���	#BF4B4B4B.��5      �� $ 
Horizontal=�6   
�������$��16(�6Q   .+�5      �� $��Center=�6   $�� Отменить ��69   $��Center=�6K   5      5      �� 5   
   $��Center=�6   $��	0,109,0,0q�61   $	��Top=�6D   ���	#7FB8B8B8$	��338��6s   $��61��6   $��Italic$�6�   ��5      }� �� 5   *   $g�aHint: Если вы не выберете ни один из пунктов, произойдет��6.   �� 6p   �� 6|   $i�cостановка торрента на текущем сеансе, при перезапуске��6�   �� 6�   �� 6�   $,�&вы снова его увидите.��6�   6�   5      5      