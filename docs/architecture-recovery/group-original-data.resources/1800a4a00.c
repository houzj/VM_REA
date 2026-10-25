
void FUN_1800a4a00(longlong param_1)

{
  undefined1 auStack_78 [48];
  undefined4 uStack_48;
  undefined8 uStack_40;
  undefined1 uStack_38;
  undefined7 uStack_37;
  undefined8 uStack_28;
  ulonglong uStack_20;
  ulonglong uStack_18;
  
  uStack_40 = 0xfffffffffffffffe;
  uStack_18 = DAT_1802a5230 ^ (ulonglong)auStack_78;
  uStack_48 = 0;
  uStack_20 = 0xf;
  uStack_28 = 0;
  uStack_38 = 0;
  FUN_180006300(&uStack_38,&DAT_180227a34,5);
  FUN_1800a4a90(param_1,param_1 + 0x170,&uStack_38);
  if (0xf < uStack_20) {
    __3_YAXPEAX_Z(CONCAT71(uStack_37,uStack_38));
  }
  FUN_1801e8810(uStack_18 ^ (ulonglong)auStack_78);
  return;
}

