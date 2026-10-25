
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800a4a90(longlong param_1,longlong *param_2,undefined8 param_3)

{
  undefined4 uVar1;
  undefined8 *puVar2;
  undefined8 *puVar3;
  ulonglong uVar4;
  char cVar5;
  int iVar6;
  undefined8 *puVar7;
  longlong *plVar8;
  undefined8 uVar9;
  undefined8 uVar10;
  longlong *plVar11;
  undefined1 auStack_1e8 [32];
  undefined4 uStack_1c8;
  undefined4 uStack_1c0;
  undefined8 uStack_1b8;
  undefined8 uStack_1b0;
  undefined1 *puStack_1a8;
  longlong *plStack_1a0;
  undefined8 *puStack_198;
  undefined8 uStack_188;
  undefined8 *puStack_180;
  undefined8 uStack_178;
  undefined8 uStack_170;
  longlong alStack_168 [2];
  undefined8 uStack_158;
  ulonglong uStack_150;
  undefined8 auStack_148 [3];
  ulonglong uStack_130;
  undefined8 auStack_128 [3];
  ulonglong uStack_110;
  undefined8 auStack_108 [3];
  ulonglong uStack_f0;
  undefined8 auStack_e8 [3];
  ulonglong uStack_d0;
  undefined8 auStack_c8 [3];
  ulonglong uStack_b0;
  undefined8 auStack_a8 [3];
  ulonglong uStack_90;
  undefined8 auStack_88 [3];
  ulonglong uStack_70;
  undefined8 auStack_68 [3];
  ulonglong uStack_50;
  ulonglong uStack_48;
  
  uStack_170 = 0xfffffffffffffffe;
  uStack_48 = DAT_1802a5230 ^ (ulonglong)auStack_1e8;
  puStack_180 = (undefined8 *)0x0;
  uStack_178 = 0;
  uStack_188 = param_3;
  puStack_180 = (undefined8 *)FUN_1800703f0();
  puVar2 = (undefined8 *)*param_2;
  puVar3 = (undefined8 *)*puVar2;
  do {
    if (puVar3 == puVar2) {
      FUN_180092540(&puStack_180,&uStack_188,*puStack_180,puStack_180);
      __3_YAXPEAX_Z(puStack_180);
      FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_1e8);
      return;
    }
    uVar1 = *(undefined4 *)(puVar3 + 2);
    uStack_150 = 0xf;
    uStack_158 = 0;
    alStack_168[0]._0_1_ = 0;
    FUN_180006470(alStack_168,puVar3 + 3,0,0xffffffffffffffff);
    switch(uVar1) {
    case 1:
      iVar6 = FUN_180010450(param_1 + 0x128,alStack_168);
      cVar5 = FUN_18001dbc0(0);
      if (cVar5 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_a8,iVar6);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = alStack_168;
        if (0xf < uStack_150) {
          plVar11 = (longlong *)CONCAT71(alStack_168[0]._1_7_,(undefined1)alStack_168[0]);
        }
        uVar9 = FUN_180006450(param_3);
        uVar10 = FUN_18001db50();
        uStack_1b8 = *(undefined8 *)(param_1 + 0xf8);
        uStack_1c0 = *(undefined4 *)(param_1 + 0x2c);
        uStack_1c8 = 0x36a;
        uStack_1b0 = uVar9;
        puStack_1a8 = (undefined1 *)plVar11;
        plStack_1a0 = plVar8;
        FUN_180015b40(uVar10,0);
        uVar9 = auStack_a8[0];
        uVar4 = uStack_90;
joined_r0x0001800a4d78:
        param_3 = uStack_188;
        if (0xf < uVar4) {
          __3_YAXPEAX_Z(uVar9);
          param_3 = uStack_188;
        }
      }
      break;
    case 2:
      iVar6 = FUN_180010ab0(param_1 + 0x128,alStack_168);
      cVar5 = FUN_18001dbc0(0);
      if (cVar5 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_108,iVar6);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = alStack_168;
        if (0xf < uStack_150) {
          plVar11 = (longlong *)CONCAT71(alStack_168[0]._1_7_,(undefined1)alStack_168[0]);
        }
        uVar9 = FUN_180006450(param_3);
        uVar10 = FUN_18001db50();
        uStack_1b8 = *(undefined8 *)(param_1 + 0xf8);
        uStack_1c0 = *(undefined4 *)(param_1 + 0x2c);
        uStack_1c8 = 0x36e;
        uStack_1b0 = uVar9;
        puStack_1a8 = (undefined1 *)plVar11;
        plStack_1a0 = plVar8;
        FUN_180015b40(uVar10,0);
        uVar9 = auStack_108[0];
        uVar4 = uStack_f0;
        goto joined_r0x0001800a4d78;
      }
      break;
    case 3:
      iVar6 = FUN_1800117a0(param_1 + 0x128,alStack_168);
      cVar5 = FUN_18001dbc0(0);
      if (cVar5 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_c8,iVar6);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = alStack_168;
        if (0xf < uStack_150) {
          plVar11 = (longlong *)CONCAT71(alStack_168[0]._1_7_,(undefined1)alStack_168[0]);
        }
        uVar9 = FUN_180006450(param_3);
        uVar10 = FUN_18001db50();
        uStack_1b8 = *(undefined8 *)(param_1 + 0xf8);
        uStack_1c0 = *(undefined4 *)(param_1 + 0x2c);
        uStack_1c8 = 0x376;
        uStack_1b0 = uVar9;
        puStack_1a8 = (undefined1 *)plVar11;
        plStack_1a0 = plVar8;
        FUN_180015b40(uVar10,0);
        uVar9 = auStack_c8[0];
        uVar4 = uStack_b0;
        goto joined_r0x0001800a4d78;
      }
      break;
    case 4:
      iVar6 = FUN_180011f60(param_1 + 0x128,alStack_168);
      cVar5 = FUN_18001dbc0(0);
      if (cVar5 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_88,iVar6);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = alStack_168;
        if (0xf < uStack_150) {
          plVar11 = (longlong *)CONCAT71(alStack_168[0]._1_7_,(undefined1)alStack_168[0]);
        }
        uVar9 = FUN_180006450(param_3);
        uVar10 = FUN_18001db50();
        uStack_1b8 = *(undefined8 *)(param_1 + 0xf8);
        uStack_1c0 = *(undefined4 *)(param_1 + 0x2c);
        uStack_1c8 = 0x37a;
        uStack_1b0 = uVar9;
        puStack_1a8 = (undefined1 *)plVar11;
        plStack_1a0 = plVar8;
        FUN_180015b40(uVar10,0);
        uVar9 = auStack_88[0];
        uVar4 = uStack_70;
        goto joined_r0x0001800a4d78;
      }
      break;
    case 5:
      iVar6 = FUN_180012600(param_1 + 0x128,alStack_168);
      cVar5 = FUN_18001dbc0(0);
      if (cVar5 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_e8,iVar6);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = alStack_168;
        if (0xf < uStack_150) {
          plVar11 = (longlong *)CONCAT71(alStack_168[0]._1_7_,(undefined1)alStack_168[0]);
        }
        uVar9 = FUN_180006450(param_3);
        uVar10 = FUN_18001db50();
        uStack_1b8 = *(undefined8 *)(param_1 + 0xf8);
        uStack_1c0 = *(undefined4 *)(param_1 + 0x2c);
        uStack_1c8 = 0x37e;
        uStack_1b0 = uVar9;
        puStack_1a8 = (undefined1 *)plVar11;
        plStack_1a0 = plVar8;
        FUN_180015b40(uVar10,0);
        uVar9 = auStack_e8[0];
        uVar4 = uStack_d0;
        goto joined_r0x0001800a4d78;
      }
      break;
    case 6:
      iVar6 = FUN_180012c70(param_1 + 0x128,alStack_168);
      cVar5 = FUN_18001dbc0(0);
      if (cVar5 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_128,iVar6);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = alStack_168;
        if (0xf < uStack_150) {
          plVar11 = (longlong *)CONCAT71(alStack_168[0]._1_7_,(undefined1)alStack_168[0]);
        }
        uVar9 = FUN_180006450(param_3);
        uVar10 = FUN_18001db50();
        uStack_1b8 = *(undefined8 *)(param_1 + 0xf8);
        uStack_1c0 = *(undefined4 *)(param_1 + 0x2c);
        uStack_1c8 = 0x382;
        uStack_1b0 = uVar9;
        puStack_1a8 = (undefined1 *)plVar11;
        plStack_1a0 = plVar8;
        FUN_180015b40(uVar10,0);
        uVar9 = auStack_128[0];
        uVar4 = uStack_110;
        goto joined_r0x0001800a4d78;
      }
      break;
    case 7:
      iVar6 = FUN_180011140(param_1 + 0x128,alStack_168);
      cVar5 = FUN_18001dbc0(0);
      if (cVar5 != '\0') {
        plVar8 = (longlong *)FUN_18008f750(auStack_68,iVar6);
        if (0xf < (ulonglong)plVar8[3]) {
          plVar8 = (longlong *)*plVar8;
        }
        plVar11 = alStack_168;
        if (0xf < uStack_150) {
          plVar11 = (longlong *)CONCAT71(alStack_168[0]._1_7_,(undefined1)alStack_168[0]);
        }
        uVar9 = FUN_180006450(param_3);
        uVar10 = FUN_18001db50();
        uStack_1b8 = *(undefined8 *)(param_1 + 0xf8);
        uStack_1c0 = *(undefined4 *)(param_1 + 0x2c);
        uStack_1c8 = 0x372;
        uStack_1b0 = uVar9;
        puStack_1a8 = (undefined1 *)plVar11;
        plStack_1a0 = plVar8;
        FUN_180015b40(uVar10,0);
        uVar9 = auStack_68[0];
        uVar4 = uStack_50;
        goto joined_r0x0001800a4d78;
      }
      break;
    default:
      iVar6 = -0x1fffffff;
      cVar5 = FUN_18001dbc0(4);
      if (cVar5 != '\0') {
        puVar7 = (undefined8 *)FUN_18008f750(auStack_148,0xe0000001);
        if (0xf < (ulonglong)puVar7[3]) {
          puVar7 = (undefined8 *)*puVar7;
        }
        plVar8 = alStack_168;
        if (0xf < uStack_150) {
          plVar8 = (longlong *)CONCAT71(alStack_168[0]._1_7_,(undefined1)alStack_168[0]);
        }
        if ((DAT_1802b1218 & 1) == 0) {
          DAT_1802b1218 = DAT_1802b1218 | 1;
          _DAT_1802b1230 = 1;
          DAT_1802b1238 = 0;
          DAT_1802b1240 = 0;
          uRam00000001802b1248 = 0;
          DAT_1802b1250 = 0;
          uRam00000001802b1258 = 0;
          DAT_1802b1260 = 0;
          uRam00000001802b1268 = 0;
          _DAT_1802b1270 = 0;
          FUN_1801e8930(&DAT_180202850);
        }
        uStack_1b0 = FUN_180006450(uStack_188);
        puStack_1a8 = (undefined1 *)CONCAT44(puStack_1a8._4_4_,uVar1);
        uStack_1b8 = *(undefined8 *)(param_1 + 0xf8);
        uStack_1c0 = *(undefined4 *)(param_1 + 0x2c);
        uStack_1c8 = 0x386;
        plStack_1a0 = plVar8;
        puStack_198 = puVar7;
        FUN_180015b40(&DAT_1802b1230,4);
        if (0xf < uStack_130) {
          __3_YAXPEAX_Z(auStack_148[0]);
        }
      }
      goto LAB_1800a5232;
    }
    if (iVar6 != 0) {
      if (iVar6 == -0x1ffffcec) {
        cVar5 = FUN_18001dbc0(1);
        if (cVar5 != '\0') {
          plVar8 = (longlong *)FUN_18008f750(auStack_128,0xe0000314);
          if (0xf < (ulonglong)plVar8[3]) {
            plVar8 = (longlong *)*plVar8;
          }
          plVar11 = alStack_168;
          if (0xf < uStack_150) {
            plVar11 = (longlong *)CONCAT71(alStack_168[0]._1_7_,(undefined1)alStack_168[0]);
          }
          uVar9 = FUN_180006450(param_3);
          uVar10 = FUN_18001db50();
          uStack_1b8 = *(undefined8 *)(param_1 + 0xf8);
          uStack_1c0 = *(undefined4 *)(param_1 + 0x2c);
          uStack_1c8 = 0x38e;
          uStack_1b0 = uVar9;
          puStack_1a8 = (undefined1 *)plVar11;
          plStack_1a0 = plVar8;
          FUN_180015b40(uVar10,1);
          uVar9 = auStack_128[0];
          uVar4 = uStack_110;
joined_r0x0001800a52c9:
          if (0xf < uVar4) {
            __3_YAXPEAX_Z(uVar9);
          }
        }
      }
      else {
LAB_1800a5232:
        cVar5 = FUN_18001dbc0(4);
        if (cVar5 != '\0') {
          plVar8 = (longlong *)FUN_18008f750(auStack_148,iVar6);
          if (0xf < (ulonglong)plVar8[3]) {
            plVar8 = (longlong *)*plVar8;
          }
          plVar11 = alStack_168;
          if (0xf < uStack_150) {
            plVar11 = (longlong *)CONCAT71(alStack_168[0]._1_7_,(undefined1)alStack_168[0]);
          }
          uVar9 = FUN_180006450(uStack_188);
          uVar10 = FUN_18001db50();
          uStack_1b8 = *(undefined8 *)(param_1 + 0xf8);
          uStack_1c0 = *(undefined4 *)(param_1 + 0x2c);
          uStack_1c8 = 0x392;
          uStack_1b0 = uVar9;
          puStack_1a8 = (undefined1 *)plVar11;
          plStack_1a0 = plVar8;
          FUN_180015b40(uVar10,4);
          uVar9 = auStack_148[0];
          uVar4 = uStack_130;
          goto joined_r0x0001800a52c9;
        }
      }
    }
    if (0xf < uStack_150) {
      __3_YAXPEAX_Z(CONCAT71(alStack_168[0]._1_7_,(undefined1)alStack_168[0]));
    }
    puVar3 = (undefined8 *)*puVar3;
    param_3 = uStack_188;
  } while( true );
}

