
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_M_SubScribe(int *param_1)

{
  int iVar1;
  char cVar2;
  uint uVar3;
  undefined8 uVar4;
  undefined8 *puVar5;
  int *piVar6;
  ulonglong uVar7;
  ulonglong uVar8;
  ulonglong uVar9;
  ulonglong uVar10;
  undefined1 auStack_188 [32];
  undefined4 uStack_168;
  undefined8 *puStack_160;
  undefined1 auStack_158 [272];
  undefined1 uStack_48;
  undefined7 uStack_47;
  undefined8 uStack_38;
  ulonglong uStack_30;
  ulonglong uStack_28;
  
                    /* 0x13b480  234  VM_M_SubScribe */
  uStack_28 = _DAT_1802a5230 ^ (ulonglong)auStack_188;
  if (param_1 != (int *)0x0) {
    cVar2 = FUN_18001dbc0(2);
    if (cVar2 != '\0') {
      puStack_160 = (undefined8 *)FUN_1801066e0(&uStack_48,param_1);
      if (0xf < (ulonglong)puStack_160[3]) {
        puStack_160 = (undefined8 *)*puStack_160;
      }
      uVar4 = FUN_18001db50();
      uStack_168 = 0x30b;
      FUN_180015b40(uVar4,2,&UNK_18021fa18,&UNK_180236e50);
      if (0xf < uStack_30) {
        __3_YAXPEAX_Z(CONCAT71(uStack_47,uStack_48));
      }
    }
    uVar10 = 0;
    uVar9 = uVar10;
    if (0 < *param_1) {
      piVar6 = param_1 + 2;
      uVar7 = uVar10;
      uVar8 = uVar10;
      while ((longlong)uVar8 < 0x80) {
        iVar1 = piVar6[-1];
        uStack_30 = 0xf;
        uStack_38 = 0;
        uStack_48 = 0;
        uVar9 = uVar10;
        if ((char)*piVar6 != '\0') {
          uVar9 = 0xffffffffffffffff;
          do {
            uVar9 = uVar9 + 1;
          } while (*(char *)((longlong)piVar6 + uVar9) != '\0');
        }
        FUN_180006300(&uStack_48,piVar6,uVar9);
        FUN_180143f80(piVar6 + 0x30,auStack_158);
        if ((_DAT_1802b2cd8 & 1) == 0) {
          _DAT_1802b2cd8 = _DAT_1802b2cd8 | 1;
          FUN_1800facf0();
          FUN_1801e8930(FUN_180202b30);
        }
        uVar3 = FUN_1800fadb0(&DAT_1802b2ce0,iVar1,&uStack_48,auStack_158);
        uVar9 = (ulonglong)uVar3;
        if (uVar3 != 0) {
          if (0xf < uStack_30) {
            __3_YAXPEAX_Z(CONCAT71(uStack_47,uStack_48));
          }
          break;
        }
        if (0xf < uStack_30) {
          __3_YAXPEAX_Z(CONCAT71(uStack_47,uStack_48));
        }
        uVar3 = (int)uVar7 + 1;
        uVar7 = (ulonglong)uVar3;
        uVar8 = uVar8 + 1;
        piVar6 = piVar6 + 0xb5;
        if (*param_1 <= (int)uVar3) break;
      }
    }
    cVar2 = FUN_18001dbc0(2);
    if (cVar2 != '\0') {
      puVar5 = (undefined8 *)FUN_18008f750(&uStack_48,uVar9);
      if (0xf < (ulonglong)puVar5[3]) {
        puVar5 = (undefined8 *)*puVar5;
      }
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
      uStack_168 = 0x31e;
      puStack_160 = puVar5;
      FUN_180015b40(&DAT_1802b1230,2,&UNK_180234258,&UNK_180236e50);
      if (0xf < uStack_30) {
        __3_YAXPEAX_Z(CONCAT71(uStack_47,uStack_48));
      }
    }
  }
  FUN_1801e8810(uStack_28 ^ (ulonglong)auStack_188);
  return;
}

