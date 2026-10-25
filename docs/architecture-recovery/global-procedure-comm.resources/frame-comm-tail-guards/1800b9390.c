
undefined8 FUN_1800b9390(longlong param_1,longlong param_2,undefined8 param_3,int param_4)

{
  char cVar1;
  bool bVar2;
  char cVar3;
  undefined8 uVar4;
  char *pcVar5;
  longlong lVar6;
  longlong lVar7;
  longlong lVar8;
  int iVar9;
  
  lVar6 = *(longlong *)(param_1 + 0x140);
  lVar7 = (longlong)param_4;
  if (lVar6 == 0) {
    cVar3 = FUN_18001dbc0(4);
    if (cVar3 != '\0') {
      uVar4 = FUN_18001db50();
      FUN_180015b40(uVar4,4,&UNK_180229d50,&UNK_180229ca0,0xfd,*(undefined4 *)(param_1 + 0x2c));
    }
    uVar4 = 0xe0000302;
  }
  else {
    iVar9 = 0;
    bVar2 = false;
    if (0 < *(int *)(lVar6 + 0x58490)) {
      do {
        pcVar5 = (char *)(lVar6 + 0x58498 + (longlong)iVar9 * 0x118);
        lVar8 = param_2 - (longlong)pcVar5;
        do {
          cVar3 = *pcVar5;
          cVar1 = pcVar5[lVar8];
          if (cVar3 != cVar1) break;
          pcVar5 = pcVar5 + 1;
        } while (cVar1 != '\0');
        if (cVar3 == cVar1) {
          bVar2 = true;
          break;
        }
        iVar9 = iVar9 + 1;
        bVar2 = false;
      } while (iVar9 < *(int *)(lVar6 + 0x58490));
      if (0x7f < iVar9) {
        cVar3 = FUN_18001dbc0(4);
        if (cVar3 != '\0') {
          uVar4 = FUN_18001db50();
          FUN_180015b40(uVar4,4,&UNK_180229d00,&UNK_180229ca0,0x10f,*(undefined4 *)(param_1 + 0x2c))
          ;
        }
        return 0xe000001a;
      }
    }
    lVar6 = (longlong)iVar9 * 0x118 + lVar6;
    if ((bVar2) && (*(longlong *)(lVar6 + 0x585a0) != 0)) {
      ___V_YAXPEAX_Z();
      *(undefined8 *)(lVar6 + 0x585a0) = 0;
      *(undefined4 *)(lVar6 + 0x585a8) = 0;
    }
    lVar8 = func_0x0001801e8c60(lVar7,0x1802b0a11);
    *(longlong *)(lVar6 + 0x585a0) = lVar8;
    if (lVar8 == 0) {
      cVar3 = FUN_18001dbc0(4);
      if (cVar3 != '\0') {
        uVar4 = FUN_18001db50();
        FUN_180015b40(uVar4,4,&UNK_180229c08,&UNK_180229ca0,0x11e,*(undefined4 *)(param_1 + 0x2c));
      }
      uVar4 = 0xe0000002;
    }
    else {
      *(int *)(lVar6 + 0x585a8) = param_4;
      memcpy_s(lVar8,lVar7,param_3,lVar7);
      uVar4 = 0;
    }
  }
  return uVar4;
}

