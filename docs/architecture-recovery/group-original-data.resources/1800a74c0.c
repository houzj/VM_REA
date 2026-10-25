
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800a74c0(longlong param_1,longlong *param_2,longlong *param_3)

{
  longlong lVar1;
  char cVar2;
  int iVar3;
  longlong lVar4;
  longlong *plVar5;
  longlong *plVar6;
  undefined4 uVar7;
  undefined4 extraout_XMM0_Da;
  undefined1 auStack_2b8 [32];
  undefined4 *puStack_298;
  undefined4 uStack_290;
  longlong lStack_288;
  longlong *plStack_278;
  undefined8 uStack_270;
  undefined4 uStack_268;
  undefined1 uStack_264;
  undefined2 uStack_263;
  undefined1 uStack_261;
  undefined8 uStack_260;
  undefined4 uStack_258;
  undefined4 uStack_254;
  undefined1 auStack_250 [16];
  undefined8 uStack_240;
  undefined8 uStack_238;
  undefined8 uStack_230;
  undefined8 uStack_228;
  undefined8 uStack_220;
  undefined8 uStack_218;
  undefined8 uStack_210;
  undefined8 uStack_208;
  undefined8 uStack_200;
  undefined8 uStack_1f8;
  undefined8 uStack_1f0;
  undefined8 uStack_1e8;
  undefined8 uStack_1e0;
  undefined8 uStack_1d8;
  undefined8 uStack_1d0;
  undefined8 uStack_1c8;
  undefined1 auStack_1c0 [376];
  ulonglong uStack_48;
  
  uStack_270 = 0xfffffffffffffffe;
  uStack_48 = DAT_1802a5230 ^ (ulonglong)auStack_2b8;
  param_2 = (longlong *)*param_2;
  plVar5 = (longlong *)*param_2;
  if (plVar5 != param_2) {
    plStack_278 = (longlong *)auStack_250;
    do {
      uStack_238 = 0xf;
      uStack_240 = 0;
      auStack_250[0] = 0;
      uStack_230 = 0;
      uStack_228 = 0;
      uStack_220 = 0;
      uStack_218 = 0;
      uStack_210 = 0;
      uStack_208 = 0;
      uStack_200 = 0;
      uStack_1f8 = 0;
      uStack_1f0 = 0;
      uStack_1e8 = 0;
      uStack_1e0 = 0;
      uStack_1d8 = 0;
      uStack_1d0 = 0;
      uStack_1c8 = 0;
      FUN_180006300(auStack_250,&DAT_18021cf35,0);
      uVar7 = FUN_1800a0310(auStack_1c0);
      uStack_268 = *(undefined4 *)(param_1 + 0x130);
      uStack_264 = 0;
      uStack_263 = 0;
      uStack_261 = 0;
      uStack_260 = *(undefined8 *)(param_1 + 0xf8);
      lVar1 = plVar5[2];
      plVar6 = plVar5 + 3;
      if ((DAT_1802b2b28 & 1) == 0) {
        DAT_1802b2b28 = DAT_1802b2b28 | 1;
        uVar7 = FUN_1801e8930(&DAT_1802029f0);
      }
      puStack_298 = &uStack_258;
      iVar3 = FUN_18012e200(uVar7,&uStack_268,(int)lVar1,plVar6);
      if (iVar3 == 0) {
        uStack_258 = (undefined4)plVar5[2];
        uStack_254 = *(undefined4 *)((longlong)plVar5 + 0x14);
        uVar7 = extraout_XMM0_Da;
        if ((longlong *)auStack_250 != plVar6) {
          uVar7 = FUN_180006470(auStack_250,plVar6,0,0xffffffffffffffff);
        }
        lVar1 = *param_3;
        lVar4 = FUN_1800a9270(uVar7,lVar1,*(undefined8 *)(lVar1 + 8),&uStack_258);
        if (param_3[1] == 0x7a44c6afc2dd9b) {
          __Xlength_error_std__YAXPEBD_Z(&DAT_18021d318);
          break;
        }
        param_3[1] = param_3[1] + 1;
        *(longlong *)(lVar1 + 8) = lVar4;
        **(longlong **)(lVar4 + 8) = lVar4;
      }
      else {
        cVar2 = FUN_18001dbc0(4);
        if (cVar2 != '\0') {
          if (0xf < (ulonglong)plVar5[6]) {
            plVar6 = (longlong *)*plVar6;
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
          puStack_298 = (undefined4 *)CONCAT44(puStack_298._4_4_,0x5dd);
          uStack_290 = (int)lVar1;
          lStack_288 = (longlong)plVar6;
          FUN_180015b40(&DAT_1802b1230,4,&DAT_180228288,&DAT_180228238);
        }
      }
      FUN_1800a77a0(auStack_1c0);
      FUN_1800a03a0(auStack_250);
      plVar5 = (longlong *)*plVar5;
    } while (plVar5 != param_2);
  }
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_2b8);
  return;
}

