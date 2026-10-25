
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180047570(longlong param_1)

{
  longlong lVar1;
  char cVar2;
  int iVar3;
  undefined8 uVar4;
  longlong lVar5;
  undefined1 auStack_1c8 [32];
  undefined4 uStack_1a8;
  undefined1 auStack_198 [16];
  undefined8 uStack_188;
  undefined8 uStack_180;
  undefined1 *puStack_178;
  longlong lStack_170;
  undefined8 uStack_168;
  undefined1 uStack_160;
  undefined7 uStack_15f;
  undefined8 uStack_150;
  ulonglong uStack_148;
  undefined4 auStack_138 [68];
  ulonglong uStack_28;
  
  uStack_168 = 0xfffffffffffffffe;
  uStack_28 = _DAT_1802a5230 ^ (ulonglong)auStack_1c8;
  cVar2 = FUN_18001dbc0(2);
  if (cVar2 != '\0') {
    uVar4 = FUN_18001db50();
    uStack_1a8 = 0x726;
    FUN_180015b40(uVar4,2,&UNK_180220178,&UNK_180220158);
  }
  if (*(char *)(param_1 + 0x20) == '\0') {
    if (*(char *)(param_1 + 0x21) == '\0') {
      if (*(char *)(param_1 + 0x22) == '\0') {
        lVar1 = param_1 + 0x50;
        lStack_170 = lVar1;
        iVar3 = _Mtx_lock(lVar1);
        if (iVar3 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar3);
        }
        *(undefined1 *)(param_1 + 0x22) = 1;
        auStack_138[0] = 1;
        if (_DAT_1802a5258 != (code *)0x0) {
          (*_DAT_1802a5258)(8,auStack_138,_DAT_1802b1180);
        }
        FUN_18003bcc0(3);
        lVar5 = FUN_18008e5d0();
        if ((((*(longlong *)(lVar5 + 0x148) != 0) &&
             (iVar3 = WaitForSingleObject(*(longlong *)(lVar5 + 0x148),0xffffffff), iVar3 != 0)) &&
            (iVar3 != 0x80)) && (iVar3 != 0x102)) {
          GetLastError();
        }
        FUN_1800441c0(param_1,0xffffffff);
        FUN_180046b20();
        uVar4 = FUN_18008eba0();
        FUN_1800b47f0(uVar4);
        uVar4 = FUN_18008ec20();
        FUN_18009d800(uVar4);
        uVar4 = FUN_18008ed60();
        FUN_1800bdf40(uVar4);
        uVar4 = FUN_18008eb60();
        FUN_1800edbf0(uVar4);
        FUN_1801e5430(0x1802b1178);
        FUN_18003c570();
        FUN_1801e5390(0x1802b1178);
        uVar4 = FUN_18008e650();
        FUN_1800b7000(uVar4);
        lVar5 = FUN_18008e610();
        iVar3 = _Mtx_lock(lVar5 + 0x130);
        if (iVar3 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar3);
        }
        *(undefined8 *)(lVar5 + 0x128) = 0;
        iVar3 = _Mtx_unlock(lVar5 + 0x130);
        if (iVar3 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar3);
        }
        uStack_148 = 0xf;
        uStack_150 = 0;
        uStack_160 = 0;
        FUN_180006300(&uStack_160,&UNK_1802201f8,8);
        puStack_178 = auStack_198;
        uStack_180 = 0xf;
        uStack_188 = 0;
        auStack_198[0] = 0;
        FUN_180006470(auStack_198,&uStack_160,0,0xffffffffffffffff);
        if ((_DAT_1802b1888 & 1) == 0) {
          _DAT_1802b1888 = _DAT_1802b1888 | 1;
          FUN_180107fc0();
          FUN_1801e8930(&UNK_180202980);
        }
        FUN_180108770(&DAT_1802b1890,10999,auStack_198);
        if (0xf < uStack_148) {
          __3_YAXPEAX_Z(CONCAT71(uStack_15f,uStack_160));
        }
        uStack_148 = 0xf;
        uStack_150 = 0;
        uStack_160 = 0;
        FUN_180006300(&uStack_160,&UNK_18021eee8,0x1c);
        puStack_178 = auStack_198;
        uStack_180 = 0xf;
        uStack_188 = 0;
        auStack_198[0] = 0;
        FUN_180006470(auStack_198,&uStack_160,0,0xffffffffffffffff);
        if ((_DAT_1802b1888 & 1) == 0) {
          _DAT_1802b1888 = _DAT_1802b1888 | 1;
          FUN_180107fc0();
          FUN_1801e8930(&UNK_180202980);
        }
        FUN_180108770(&DAT_1802b1890,100000,auStack_198);
        if (0xf < uStack_148) {
          __3_YAXPEAX_Z(CONCAT71(uStack_15f,uStack_160));
        }
        uStack_148 = 0xf;
        uStack_150 = 0;
        uStack_160 = 0;
        *(undefined1 *)(param_1 + 0x22) = 0;
        auStack_138[0] = 2;
        if (_DAT_1802a5258 != (code *)0x0) {
          (*_DAT_1802a5258)(8,auStack_138,_DAT_1802b1180);
        }
        cVar2 = FUN_18001dbc0(2);
        if (cVar2 != '\0') {
          uVar4 = FUN_18001db50();
          uStack_1a8 = 0x75b;
          FUN_180015b40(uVar4,2,&UNK_180220208,&UNK_180220158);
        }
        iVar3 = _Mtx_unlock(lVar1);
        if (iVar3 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar3);
        }
      }
      else {
        cVar2 = FUN_18001dbc0(4);
        if (cVar2 != '\0') {
          uVar4 = FUN_18001db50();
          uStack_1a8 = 0x736;
          FUN_180015b40(uVar4,4,&UNK_1802201d8,&UNK_180220158);
        }
      }
    }
    else {
      cVar2 = FUN_18001dbc0(4);
      if (cVar2 != '\0') {
        uVar4 = FUN_18001db50();
        uStack_1a8 = 0x730;
        FUN_180015b40(uVar4,4,&UNK_1802201b8,&UNK_180220158);
      }
    }
  }
  else {
    cVar2 = FUN_18001dbc0(4);
    if (cVar2 != '\0') {
      uVar4 = FUN_18001db50();
      uStack_1a8 = 0x72a;
      FUN_180015b40(uVar4,4,&UNK_180220198,&UNK_180220158);
    }
  }
  FUN_1801e8810(uStack_28 ^ (ulonglong)auStack_1c8);
  return;
}

