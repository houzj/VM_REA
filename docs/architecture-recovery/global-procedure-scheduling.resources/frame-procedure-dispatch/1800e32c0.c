
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800e32c0(longlong param_1)

{
  undefined8 *puVar1;
  undefined8 *puVar2;
  longlong *plVar3;
  char cVar4;
  uint uVar5;
  int iVar6;
  longlong *plVar7;
  
  puVar1 = *(undefined8 **)(param_1 + 8);
  puVar2 = (undefined8 *)*puVar1;
  do {
    if (puVar2 == puVar1) {
      plVar3 = *(longlong **)(param_1 + 8);
      plVar7 = (longlong *)*plVar3;
      *plVar3 = (longlong)plVar3;
      *(longlong *)(*(longlong *)(param_1 + 8) + 8) = *(longlong *)(param_1 + 8);
      *(undefined8 *)(param_1 + 0x10) = 0;
      if (plVar7 != *(longlong **)(param_1 + 8)) {
        do {
          plVar7 = (longlong *)*plVar7;
          __3_YAXPEAX_Z();
        } while (plVar7 != (longlong *)*(longlong *)(param_1 + 8));
      }
      return;
    }
    uVar5 = WaitForSingleObject(puVar2[2],0xffffffff);
    if ((uVar5 & 0xffffff7f) == 0) {
LAB_1800e33c7:
      if ((_DAT_1802b3230 & 1) == 0) {
        _DAT_1802b3230 = _DAT_1802b3230 | 1;
        FUN_18007fa60();
        FUN_1801e8930(&UNK_180202f50);
      }
      FUN_180080dc0(0x1802b3240);
    }
    else {
      if (uVar5 == 0x102) {
        iVar6 = 0x102;
      }
      else {
        iVar6 = GetLastError();
        if (iVar6 == 0) goto LAB_1800e33c7;
      }
      cVar4 = FUN_18001dbc0(4);
      if (cVar4 != '\0') {
        if ((_DAT_1802b1218 & 1) == 0) {
          _DAT_1802b1218 = _DAT_1802b1218 | 1;
          _DAT_1802b1230 = 1;
          _DAT_1802b1238 = 0;
          _DAT_1802b1240 = 0;
          uRam00000001802b1248 = 0;
          _DAT_1802b1250 = 0;
          uRam00000001802b1258 = 0;
          _DAT_1802b1260 = 0;
          uRam00000001802b1268 = 0;
          _DAT_1802b1270 = 0;
          FUN_1801e8930(&UNK_180202850);
        }
        FUN_180015b40(&DAT_1802b1230,4,&UNK_180224e30,&UNK_18022d6d0,0x76,iVar6);
      }
    }
    puVar2 = (undefined8 *)*puVar2;
  } while( true );
}

