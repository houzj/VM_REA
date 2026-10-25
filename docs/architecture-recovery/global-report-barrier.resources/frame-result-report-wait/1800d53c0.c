
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800d53c0(longlong param_1,int param_2,ulonglong param_3)

{
  longlong *plVar1;
  longlong *plVar2;
  int *piVar3;
  char cVar4;
  undefined8 *puVar5;
  undefined8 *puVar6;
  longlong lVar7;
  uint uVar8;
  int iVar9;
  undefined8 **ppuVar10;
  undefined8 *puVar11;
  undefined8 *puVar12;
  undefined8 *puStackX_18;
  undefined8 *puStackX_20;
  longlong in_stack_ffffffffffffff88;
  undefined4 uVar13;
  undefined8 *puStack_68;
  int *piStack_60;
  undefined8 *puStack_58;
  undefined8 uStack_50;
  longlong lStack_48;
  int aiStack_40 [2];
  ulonglong uStack_38;
  
  uStack_50 = 0xfffffffffffffffe;
  lStack_48 = param_1;
  aiStack_40[0] = param_2;
  uStack_38 = param_3;
  do {
    FUN_1801e52d0(param_1);
    puVar5 = *(undefined8 **)(param_1 + 0x60);
    cVar4 = *(char *)((longlong)puVar5[1] + 0x19);
    puVar12 = puVar5;
    puVar6 = (undefined8 *)puVar5[1];
    while (cVar4 == '\0') {
      if ((*(int *)(puVar6 + 4) < aiStack_40[0]) ||
         ((*(int *)(puVar6 + 4) <= aiStack_40[0] && ((ulonglong)puVar6[5] < uStack_38)))) {
        puVar11 = (undefined8 *)puVar6[2];
      }
      else {
        puVar11 = (undefined8 *)*puVar6;
        puVar12 = puVar6;
      }
      puVar6 = puVar11;
      cVar4 = *(char *)((longlong)puVar11 + 0x19);
    }
    if (((puVar12 == puVar5) || (aiStack_40[0] < *(int *)(puVar12 + 4))) ||
       ((aiStack_40[0] <= *(int *)(puVar12 + 4) && (uStack_38 < (ulonglong)puVar12[5])))) {
      in_stack_ffffffffffffff88 = FUN_1800d7400(param_1 + 0x60);
      FUN_1800d7150(param_1 + 0x60,&puStackX_18,puVar12,in_stack_ffffffffffffff88 + 0x20,
                    in_stack_ffffffffffffff88);
      puVar12 = puStackX_18;
    }
    if (puVar12[7] == 0) {
      puVar5 = *(undefined8 **)(param_1 + 0x90);
      cVar4 = *(char *)((longlong)puVar5[1] + 0x19);
      puVar12 = puVar5;
      puVar6 = (undefined8 *)puVar5[1];
      while (cVar4 == '\0') {
        if ((*(int *)(puVar6 + 4) < aiStack_40[0]) ||
           ((*(int *)(puVar6 + 4) <= aiStack_40[0] && ((ulonglong)puVar6[5] < uStack_38)))) {
          puVar11 = (undefined8 *)puVar6[2];
        }
        else {
          puVar11 = (undefined8 *)*puVar6;
          puVar12 = puVar6;
        }
        puVar6 = puVar11;
        cVar4 = *(char *)((longlong)puVar11 + 0x19);
      }
      if (((puVar12 == puVar5) || (aiStack_40[0] < *(int *)(puVar12 + 4))) ||
         ((aiStack_40[0] <= *(int *)(puVar12 + 4) && (uStack_38 < (ulonglong)puVar12[5])))) {
        puStack_68 = puVar5;
        ppuVar10 = &puStack_68;
      }
      else {
        ppuVar10 = &puStackX_20;
        puStackX_20 = puVar12;
      }
      if (*ppuVar10 == puVar5) {
        FUN_1801e5390(param_1);
        return;
      }
    }
    FUN_1801e5390(param_1);
    plVar1 = (longlong *)(param_1 + 0x30);
    puVar5 = (undefined8 *)*plVar1;
    cVar4 = *(char *)((longlong)puVar5[1] + 0x19);
    puVar12 = puVar5;
    puVar6 = (undefined8 *)puVar5[1];
    while (cVar4 == '\0') {
      if ((*(int *)(puVar6 + 4) < aiStack_40[0]) ||
         ((*(int *)(puVar6 + 4) <= aiStack_40[0] && ((ulonglong)puVar6[5] < uStack_38)))) {
        puVar11 = (undefined8 *)puVar6[2];
      }
      else {
        puVar11 = (undefined8 *)*puVar6;
        puVar12 = puVar6;
      }
      puVar6 = puVar11;
      cVar4 = *(char *)((longlong)puVar11 + 0x19);
    }
    if (((puVar12 == puVar5) || (aiStack_40[0] < *(int *)(puVar12 + 4))) ||
       ((aiStack_40[0] <= *(int *)(puVar12 + 4) && (uStack_38 < (ulonglong)puVar12[5])))) {
      piStack_60 = aiStack_40;
      in_stack_ffffffffffffff88 = FUN_1800d6db0(plVar1,aiStack_40[0],&piStack_60);
      FUN_1800d6df0(plVar1,&puStack_58,puVar12,in_stack_ffffffffffffff88 + 0x20,
                    in_stack_ffffffffffffff88);
      puVar12 = puStack_58;
    }
    uVar8 = WaitForSingleObject(puVar12[6],0x32);
    if (((uVar8 & 0xffffff7f) != 0) && ((uVar8 == 0x102 || (iVar9 = GetLastError(), iVar9 != 0)))) {
      if ((_DAT_1802b1218 & 1) == 0) {
        _DAT_1802b1218 = _DAT_1802b1218 | 1;
        _DAT_1802b1230 = 1;
        _DAT_1802b1238 = 0;
        _DAT_1802b1240 = (longlong *)0x0;
        uRam00000001802b1248 = 0;
        _DAT_1802b1250 = 0;
        uRam00000001802b1258 = 0;
        _DAT_1802b1260 = 0;
        uRam00000001802b1268 = 0;
        _DAT_1802b1270 = 0;
        FUN_1801e8930(&UNK_180202850);
      }
      plVar1 = _DAT_1802b1240;
      lVar7 = _DAT_1802b1238;
      if (_DAT_1802b1240 != (longlong *)0x0) {
        LOCK();
        *(int *)(_DAT_1802b1240 + 1) = (int)_DAT_1802b1240[1] + 1;
        UNLOCK();
      }
      if (lVar7 == 0) {
        if (plVar1 != (longlong *)0x0) {
          LOCK();
          plVar2 = plVar1 + 1;
          *(int *)plVar2 = (int)*plVar2 + -1;
          UNLOCK();
          if ((int)*plVar2 == 0) {
            (**(code **)*plVar1)(plVar1);
            LOCK();
            piVar3 = (int *)((longlong)plVar1 + 0xc);
            *piVar3 = *piVar3 + -1;
            UNLOCK();
            if (*piVar3 == 0) {
              (**(code **)(*plVar1 + 8))(plVar1);
            }
          }
        }
      }
      else {
        iVar9 = *(int *)(lVar7 + 0x40);
        if (plVar1 != (longlong *)0x0) {
          LOCK();
          plVar2 = plVar1 + 1;
          *(int *)plVar2 = (int)*plVar2 + -1;
          UNLOCK();
          if ((int)*plVar2 == 0) {
            (**(code **)*plVar1)(plVar1);
            LOCK();
            piVar3 = (int *)((longlong)plVar1 + 0xc);
            *piVar3 = *piVar3 + -1;
            UNLOCK();
            if (*piVar3 == 0) {
              (**(code **)(*plVar1 + 8))(plVar1);
            }
          }
        }
        uVar13 = (undefined4)((ulonglong)in_stack_ffffffffffffff88 >> 0x20);
        if (iVar9 < 4) {
          if ((_DAT_1802b1218 & 1) == 0) {
            _DAT_1802b1218 = _DAT_1802b1218 | 1;
            _DAT_1802b1230 = 1;
            _DAT_1802b1238 = 0;
            _DAT_1802b1240 = (longlong *)0x0;
            uRam00000001802b1248 = 0;
            _DAT_1802b1250 = 0;
            uRam00000001802b1258 = 0;
            _DAT_1802b1260 = 0;
            uRam00000001802b1268 = 0;
            _DAT_1802b1270 = 0;
            FUN_1801e8930(&UNK_180202850);
          }
          in_stack_ffffffffffffff88 = CONCAT44(uVar13,0x1e3);
          FUN_180015b40(&DAT_1802b1230,3,&UNK_18022c018,&UNK_18022bf90,in_stack_ffffffffffffff88,
                        param_2);
        }
      }
    }
  } while( true );
}

