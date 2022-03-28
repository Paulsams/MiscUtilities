# MiscUtilities
Different utility classes for my other packages to work with.

Also this repository contains:
1) GameObjectLayer is a structure that allows you to select a layer in the inspector without bit shift operators, that is, for comparison with GameObject.layer;
![image](https://github.com/Paulsams/MiscUtilities/blob/master/Documentation~/GameObjectLayer.gif)
2) ReadonlyField is an attribute so that you can see the value in the inspector, but not change it (but, as I understand it, it only works on non-custom types that do not have their own PropertyDrawer. And I think this because the custom PropertyDrawer redraws what ReadonlyAttribute called).
![image](https://github.com/Paulsams/MiscUtilities/blob/master/Documentation~/ReadonlyAttribute.gif)
