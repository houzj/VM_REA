
undefined8 FUN_1800b6b70(longlong param_1,int param_2)

{
  longlong *plVar1;
  longlong *plVar2;
  char cVar3;
  int iVar4;
  undefined8 uVar5;
  longlong *plVar6;
  undefined8 uVar7;
  bool bVar8;
  
  iVar4 = _Mtx_lock(param_1 + 0x38);
  if (iVar4 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar4);
  }
  uVar7 = 0;
  plVar6 = (longlong *)**(longlong **)(param_1 + 8);
  bVar8 = plVar6 == *(longlong **)(param_1 + 8);
  do {
    if (bVar8) {
LAB_1800b6c6f:
      iVar4 = _Mtx_unlock(param_1 + 0x38);
      if (iVar4 != 0) {
        __Throw_C_error_std__YAXH_Z(iVar4);
      }
      return uVar7;
    }
    if (((int *)plVar6[4] != (int *)0x0) && (*(int *)plVar6[4] == param_2)) {
      if (plVar6[5] != 0) {
        uVar7 = *(undefined8 *)(plVar6[5] + 8);
        goto LAB_1800b6c6f;
      }
      cVar3 = FUN_18001dbc0(4);
      if (cVar3 != '\0') {
        uVar5 = FUN_18001db50();
        FUN_180015b40(uVar5,4,&DAT_1802295e8,&DAT_180229660,0x152);
      }
    }
    if (*(char *)((longlong)plVar6 + 0x19) == '\0') {
      plVar1 = (longlong *)plVar6[2];
      if (*(char *)((longlong)plVar1 + 0x19) == '\0') {
        cVar3 = *(char *)(*plVar1 + 0x19);
        plVar6 = plVar1;
        plVar1 = (longlong *)*plVar1;
        while (cVar3 == '\0') {
          cVar3 = *(char *)(*plVar1 + 0x19);
          plVar6 = plVar1;
          plVar1 = (longlong *)*plVar1;
        }
      }
      else {
        cVar3 = *(char *)(plVar6[1] + 0x19);
        plVar2 = (longlong *)plVar6[1];
        plVar1 = plVar6;
        while ((plVar6 = plVar2, cVar3 == '\0' && (plVar1 == (longlong *)plVar6[2]))) {
          cVar3 = *(char *)(plVar6[1] + 0x19);
          plVar2 = (longlong *)plVar6[1];
          plVar1 = plVar6;
        }
      }
    }
    bVar8 = plVar6 == *(longlong **)(param_1 + 8);
  } while( true );
}

