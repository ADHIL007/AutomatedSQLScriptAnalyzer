   M S B A M L   `   `   `  ����M  ITestGG, Version=1.0.0.0, Culture=neutral, PublicKeyToken=edd0f331379b6a2f+clr-namespace:TestGG.PagesTestGG.Pages  5          TestGG.Pages.EditorPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ^ ZICSharpCode.AvalonEdit, Version=6.3.0.90, Culture=neutral, PublicKeyToken=9cc39be672370310?
avalonEdit.http://icsharpcode.net/sharpdevelop/avalonedit  5      &localclr-namespace:TestGG.Pages  5        N� 
Background  �#E0E0E0.��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $��3* �6   �� 5      $��1* �6   .+�5   
   �� $��0��6   $��0��6!   
����������#B0B0B0$��1q�6W    �� Padding$ 5q�6k   ._�5      '  !ICSharpCode.AvalonEdit.TextEditor  -   5          Name$ 
textEditor��5   $   $��Consolas)�5    $   $��14(�5!   $   ���#333333
�������   ShowLineNumbers . $��Stretch=�5%   $   $��Stretch=�5&   $   5'      �� 5*   
   $��0��6   $��1��6!   ���#FFFFFF���#B0B0B0$��1q�6Y   $ 5q�6m   $��0,5,0,5q�6y   ._�5+      �  5,      �� 5-      $
��Auto �6'   �� 5.      $��* �6'   .+�51      ��  �� Orientation$ 
Horizontal=�6   $��0��66   $��Center=�6F   .+�52      �� -   5         ��Name$ 
statusIcon��52      $��16��60   $��16��6;   $��0,0,5,0q�6G   �� 53      -   5         ��Name$ 	statusLog��53       $��12(�63   ���#333333$��Center=�6V   $��Ready��6q   54      �� 56      $ 
Horizontal=�6   $��1��66   $��Center=�6F   $��Right=�6a   $��0,0,10,0q�6}   .+�57      �� $��Token: ��6    $��Center=�6/   �� 58      -   5        $ 	tokenText��58   *   $��10��6    $
��Bold"�6=   $��Center=�6O   59      5:      5;      �� 5>   
   $��0��6   $��2��6%   $ 
Horizontal=�62   $��Right=�6K   $��0,5,0,0q�6g   .+�5?      �� -   5        	 ��Name$	 
analyzeBtn��5?   '   $��Analyze��6   $��14(�6;   $	��120��6I   $��35��6U   
 � Cursor$

 Hand��6a   $��5q�6�   {�.   J� Opacity$	 0.8Q�6�   ��5@      �� 5A       �� Color$ #0a3d62��6&    �� Opacity$	 0.8Q�66   5C      �� 5E      -   5        $	 FixBtn��5E   2   $��Fix & Optimize��6   $��14(�6B   $	��120��5G      $��35��6!   $

 Hand��6-   {�.  $��5q�6M   ��5H      �� 5I      $ #0a3d62��6&   $	 0.8Q�66   5K      �� 5M      -   5        $	 ApplyBtn��5M   %   $��Apply��6   $��14(�6W   $	��120��6e   $��35��6q   $

 Hand��6}   {�.  $��5q�6�   ��5N      �� 5O      $ #0a3d62��6&   $	 0.8Q�66   5Q      5R      � 5T   
   $��1��6   $��3��6   .+�5U      �� ���	#80FFFFFF���#B0B0B0$��1q�6B   $ 10q�6V   $��5,0,0,0q�6c   ._�5V      � .+�5X      � $��0��6   $��60��6,   $��5q�6;    5Y      �� 5[      $
��Auto �6,   .+�5b      �� -   5         ��Name$ DbAnalysisSwitch��5b   #    d� 	IsChecked$ FalseY�5c      $
��Left=�5d      $	��0,5q�5e      $��DB Analysis��5f      $��13(�5g      ���#333333$��1��5i      5j      �� 5m      $��Hidden=�6#   .��6J   �� -	   5         ��Name$ analysisstack��5m   k   $��5,85,5.333,0q�6U   5o      5p      5q      5r      5s      5v      5w      