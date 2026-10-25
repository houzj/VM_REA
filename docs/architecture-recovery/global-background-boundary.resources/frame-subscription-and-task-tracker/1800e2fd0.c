
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800e2fd0(longlong *param_1)

{
  undefined8 *puVar1;
  undefined8 *puVar2;
  longlong *plVar3;
  code *pcVar4;
  char cVar5;
  undefined4 uVar6;
  longlong *plVar7;
  undefined8 *puVar8;
  ulonglong uVar9;
  undefined8 *puVar10;
  undefined8 ******ppppppuVar11;
  longlong lVar12;
  longlong lVar13;
  ulonglong uVar14;
  undefined8 *puVar15;
  undefined8 *puVar16;
  undefined1 auStack_98 [32];
  undefined8 *puStack_78;
  undefined8 *puStack_70;
  undefined8 *puStack_68;
  undefined8 uStack_60;
  undefined8 *****apppppuStack_58 [2];
  undefined4 uStack_48;
  ulonglong uStack_40;
  ulonglong uStack_38;
  
  uStack_60 = 0xfffffffffffffffe;
  uStack_38 = _DAT_1802a5230 ^ (ulonglong)auStack_98;
  puStack_78 = (undefined8 *)0x0;
  puStack_70 = (undefined8 *)0x0;
  puStack_68 = (undefined8 *)0x0;
  if (param_1[2] != 0) {
    if (0x3fffffffffffffff < (ulonglong)param_1[2]) {
      __Xlength_error_std__YAXPEBD_Z(&UNK_18021d330);
      pcVar4 = (code *)swi(3);
      (*pcVar4)();
      return;
    }
    FUN_18001f0a0(&puStack_78);
  }
  puVar2 = (undefined8 *)param_1[1];
  puVar8 = (undefined8 *)*puVar2;
  puVar10 = puStack_68;
  puVar15 = puStack_70;
  puVar16 = puStack_78;
  do {
    if (puVar8 == puVar2) {
      plVar3 = (longlong *)param_1[1];
      plVar7 = (longlong *)*plVar3;
      *plVar3 = (longlong)plVar3;
      *(longlong *)(param_1[1] + 8) = param_1[1];
      param_1[2] = 0;
      puVar2 = puVar16;
      if (plVar7 != (longlong *)param_1[1]) {
        do {
          plVar7 = (longlong *)*plVar7;
          __3_YAXPEAX_Z();
        } while (plVar7 != (longlong *)param_1[1]);
      }
      for (; puVar2 != puVar15; puVar2 = (undefined8 *)((longlong)puVar2 + 4)) {
        if (*param_1 != 0) {
          FUN_1800e3590(apppppuStack_58,*(undefined4 *)puVar2);
          ppppppuVar11 = apppppuStack_58;
          if (0xf < uStack_40) {
            ppppppuVar11 = (undefined8 ******)apppppuStack_58[0];
          }
          (**(code **)(*(longlong *)*param_1 + 0x18))
                    ((longlong *)*param_1,&UNK_18022d710,ppppppuVar11,uStack_48);
          if (0xf < uStack_40) {
            __3_YAXPEAX_Z(apppppuStack_58[0]);
          }
        }
      }
      if (puVar16 != (undefined8 *)0x0) {
        __3_YAXPEAX_Z(puVar16);
      }
      FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_98);
      return;
    }
    if ((_DAT_1802b3230 & 1) == 0) {
      _DAT_1802b3230 = _DAT_1802b3230 | 1;
      FUN_18007fa60();
      FUN_1801e8930(&UNK_180202f50);
    }
    cVar5 = FUN_180080dc0(0x1802b3240,puVar8[2]);
    if (cVar5 == '\0') {
      puVar1 = puVar8 + 3;
      if ((puVar1 < puVar15) && (puVar16 <= puVar1)) {
        lVar12 = (longlong)puVar1 - (longlong)puVar16;
        if ((puVar15 == puVar10) && ((longlong)puVar10 - (longlong)puVar15 >> 2 == 0)) {
          lVar13 = (longlong)puVar15 - (longlong)puVar16 >> 2;
          if (lVar13 == 0x3fffffffffffffff) {
            __Xlength_error_std__YAXPEBD_Z(&UNK_18021d330);
            pcVar4 = (code *)swi(3);
            (*pcVar4)();
            return;
          }
          uVar14 = lVar13 + 1;
          uVar9 = (longlong)puVar10 - (longlong)puVar16 >> 2;
          if (0x3fffffffffffffff - (uVar9 >> 1) < uVar9) {
            uVar9 = 0;
          }
          else {
            uVar9 = uVar9 + (uVar9 >> 1);
          }
          if (uVar9 < uVar14) {
            uVar9 = uVar14;
          }
          FUN_18001f0a0(&puStack_78,uVar9);
          puVar10 = puStack_68;
          puVar15 = puStack_70;
          puVar16 = puStack_78;
        }
        if (puVar15 != (undefined8 *)0x0) {
          uVar6 = *(undefined4 *)((longlong)puVar16 + (lVar12 >> 2) * 4);
LAB_1800e31bd:
          *(undefined4 *)puVar15 = uVar6;
        }
      }
      else {
        if ((puVar15 == puVar10) && ((longlong)puVar10 - (longlong)puVar15 >> 2 == 0)) {
          lVar12 = (longlong)puVar15 - (longlong)puVar16 >> 2;
          if (lVar12 == 0x3fffffffffffffff) {
            __Xlength_error_std__YAXPEBD_Z(&UNK_18021d330);
            pcVar4 = (code *)swi(3);
            (*pcVar4)();
            return;
          }
          uVar14 = lVar12 + 1;
          uVar9 = (longlong)puVar10 - (longlong)puVar16 >> 2;
          if (0x3fffffffffffffff - (uVar9 >> 1) < uVar9) {
            uVar9 = 0;
          }
          else {
            uVar9 = uVar9 + (uVar9 >> 1);
          }
          if (uVar9 < uVar14) {
            uVar9 = uVar14;
          }
          FUN_18001f0a0(&puStack_78,uVar9);
          puVar10 = puStack_68;
          puVar15 = puStack_70;
          puVar16 = puStack_78;
        }
        if (puVar15 != (undefined8 *)0x0) {
          uVar6 = *(undefined4 *)puVar1;
          goto LAB_1800e31bd;
        }
      }
      puVar15 = (undefined8 *)((longlong)puVar15 + 4);
      puStack_70 = puVar15;
    }
    puVar8 = (undefined8 *)*puVar8;
  } while( true );
}

