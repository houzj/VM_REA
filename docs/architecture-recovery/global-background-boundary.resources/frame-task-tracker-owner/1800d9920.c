
longlong FUN_1800d9920(longlong *param_1,longlong param_2)

{
  int iVar1;
  longlong lVar2;
  longlong lVar3;
  longlong lStackX_8;
  
  lVar2 = FUN_1800202d0();
  if (lVar2 != *param_1) {
    iVar1 = FUN_180020a60(param_2,lVar2 + 0x20);
    if (-1 < iVar1) {
      return lVar2 + 0x40;
    }
  }
  lStackX_8 = param_2;
  lVar3 = FUN_1800d99b0(param_1);
  FUN_1800d9a40(param_1,&lStackX_8,lVar2,lVar3 + 0x20,lVar3);
  return lStackX_8 + 0x40;
}

