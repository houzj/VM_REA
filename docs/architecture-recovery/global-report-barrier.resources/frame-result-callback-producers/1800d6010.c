
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 FUN_1800d6010(ulonglong param_1,longlong *param_2,undefined8 *param_3,int *param_4)

{
  longlong *plVar1;
  int *piVar2;
  double dVar3;
  double dVar4;
  longlong *plVar5;
  char cVar6;
  int iVar7;
  int iVar8;
  undefined8 uVar9;
  undefined4 *puVar10;
  undefined8 uVar11;
  undefined4 uVar12;
  undefined4 uVar13;
  ulonglong uStackX_8;
  longlong lStackX_20;
  undefined8 in_stack_ffffffffffffff68;
  undefined4 uVar15;
  undefined8 uVar14;
  undefined8 in_stack_ffffffffffffff70;
  longlong lStack_78;
  longlong alStack_70 [3];
  longlong *plStack_58;
  
  uVar15 = (undefined4)((ulonglong)in_stack_ffffffffffffff68 >> 0x20);
  uVar12 = (undefined4)((ulonglong)in_stack_ffffffffffffff70 >> 0x20);
  alStack_70[1] = 0xfffffffffffffffe;
  uStackX_8 = param_1;
  if (_DAT_1802b12a0 == 1) {
    iVar7 = 0x40;
  }
  else {
    iVar7 = (**(code **)(*param_2 + 0x58))(param_2);
  }
  if (*param_4 < iVar7) {
    uVar9 = func_0x0001801e8774((longlong)iVar7);
    alStack_70[2] = 0;
    plStack_58 = (longlong *)0x0;
    FUN_180071e00(alStack_70 + 2,uVar9,uStackX_8 & 0xff);
    FUN_18001d870(param_3,alStack_70 + 2);
    plVar5 = plStack_58;
    if (plStack_58 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_58 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_58)(plStack_58);
        LOCK();
        piVar2 = (int *)((longlong)plVar5 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_58 + 8))();
        }
      }
    }
    *param_4 = iVar7;
  }
  if (_DAT_1802b12a0 == 1) {
    iVar7 = _snprintf_s(*param_3,(longlong)(*param_4 + -1),0xffffffffffffffff,&UNK_18022c268,param_2
                       );
    uVar15 = (undefined4)((ulonglong)param_2 >> 0x20);
    if (iVar7 == -1) {
      cVar6 = FUN_18001dbc0(4);
      if (cVar6 != '\0') {
        puVar10 = (undefined4 *)_errno();
        uVar9 = FUN_18001db50();
        FUN_180015b40(uVar9,4,&UNK_18022c2d8,&UNK_18022c280,CONCAT44(uVar15,0x283),
                      CONCAT44(uVar12,0xffffffff),*puVar10);
      }
      return 0xe000001a;
    }
  }
  else {
    cVar6 = FUN_1801a45e0(param_2,*param_3,*param_4);
    if (cVar6 == '\0') {
      cVar6 = FUN_18001dbc0(4);
      if (cVar6 != '\0') {
        uVar9 = FUN_18001db50();
        FUN_180015b40(uVar9,4,&UNK_180223dd8,&UNK_18022c280,CONCAT44(uVar15,0x28c));
      }
      return 0xe0000003;
    }
  }
  if (0 < iVar7) {
    dVar4 = 0.0;
    iVar8 = QueryPerformanceFrequency(&lStackX_20);
    if ((iVar8 == 0) || (iVar8 = QueryPerformanceCounter(&uStackX_8), iVar8 == 0)) {
      uVar12 = SUB84(_DAT_18023a7b0,0);
      uVar13 = (undefined4)((ulonglong)_DAT_18023a7b0 >> 0x20);
    }
    else {
      uVar12 = SUB84(_DAT_18023a7b0,0);
      uVar13 = (undefined4)((ulonglong)_DAT_18023a7b0 >> 0x20);
      dVar4 = ((double)(longlong)uStackX_8 / (double)lStackX_20) * _DAT_18023a7b0;
    }
    if (_DAT_1802a5260 != (code *)0x0) {
      (*_DAT_1802a5260)(*param_3,iVar7,_DAT_1802b1188);
    }
    iVar7 = QueryPerformanceFrequency(alStack_70);
    dVar3 = 0.0;
    if ((iVar7 != 0) && (iVar7 = QueryPerformanceCounter(&lStack_78), dVar3 = 0.0, iVar7 != 0)) {
      dVar3 = ((double)lStack_78 / (double)alStack_70[0]) * (double)CONCAT44(uVar13,uVar12);
    }
    if (dVar3 - dVar4 <= _DAT_18023a780) {
      cVar6 = FUN_18001dbc0(1);
      if (cVar6 == '\0') {
        return 0;
      }
      uVar9 = FUN_18001db50();
      uVar14 = CONCAT44(uVar15,0x29e);
      uVar11 = 1;
    }
    else {
      cVar6 = FUN_18001dbc0(3);
      if (cVar6 == '\0') {
        return 0;
      }
      uVar9 = FUN_18001db50();
      uVar14 = CONCAT44(uVar15,0x29a);
      uVar11 = 3;
    }
    FUN_180015b40(uVar9,uVar11,&UNK_18022c2a8,&UNK_18022c280,uVar14,dVar3 - dVar4);
  }
  return 0;
}

