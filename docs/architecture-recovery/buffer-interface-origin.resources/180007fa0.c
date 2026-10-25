
undefined8
FUN_180007fa0(longlong param_1,undefined4 param_2,longlong param_3,longlong *param_4,
             longlong *param_5,uint param_6)

{
  int *piVar1;
  uint uVar2;
  ulonglong uVar3;
  longlong *plVar4;
  char cVar5;
  DWORD DVar6;
  longlong *plVar7;
  longlong **pplVar8;
  undefined8 *puVar9;
  undefined8 uVar10;
  longlong lVar11;
  longlong extraout_RAX;
  longlong lVar12;
  int iVar13;
  longlong *plVar14;
  longlong *plVar15;
  undefined8 in_stack_ffffffffffffff58;
  undefined4 uVar16;
  undefined8 in_stack_ffffffffffffff68;
  undefined4 uVar17;
  int local_78;
  int local_74;
  undefined8 local_70;
  longlong *local_68;
  longlong *plStack_60;
  longlong *local_58;
  longlong *plStack_50;
  undefined8 local_48;
  longlong *local_40;
  
  uVar16 = (undefined4)((ulonglong)in_stack_ffffffffffffff58 >> 0x20);
  uVar17 = (undefined4)((ulonglong)in_stack_ffffffffffffff68 >> 0x20);
  local_70 = 0xfffffffffffffffe;
  if (param_3 == 0) {
    return 0xe0000001;
  }
  plVar7 = FUN_180009660((longlong *)(param_1 + 0x70),(int *)&param_6);
  uVar3 = plVar7[1];
  local_68 = (longlong *)0x0;
  plStack_60 = (longlong *)0x0;
  local_78 = 0;
  VM_M_GetBufferNodeNum(&local_78);
  if (plVar7[1] != 0) {
    plVar15 = *(longlong **)(*(longlong *)*plVar7 + 0x10);
    if ((int)plVar15[2] == 0) {
      plVar14 = *(longlong **)(*(longlong *)*plVar7 + 0x18);
      if (plVar14 != (longlong *)0x0) {
        LOCK();
        *(int *)(plVar14 + 1) = (int)plVar14[1] + 1;
        UNLOCK();
      }
      local_68 = plVar15;
      plStack_60 = plVar14;
      FUN_1800096f0(plVar7);
      goto LAB_1800081ff;
    }
  }
  iVar13 = *(int *)(param_1 + 8);
  if (*(int *)(param_1 + 8) < local_78) {
    iVar13 = local_78;
  }
  if (uVar3 < (ulonglong)(longlong)iVar13) {
    pplVar8 = (longlong **)FUN_18000a6c0(&local_48);
    local_58 = (longlong *)0x0;
    plStack_50 = (longlong *)0x0;
    if (&local_58 != pplVar8) {
      plStack_50 = pplVar8[1];
      pplVar8[1] = (longlong *)0x0;
      local_58 = *pplVar8;
      *pplVar8 = (longlong *)0x0;
    }
    local_68 = local_58;
    plStack_60 = plStack_50;
    if (local_40 != (longlong *)0x0) {
      LOCK();
      plVar15 = local_40 + 1;
      *(int *)plVar15 = (int)*plVar15 + -1;
      UNLOCK();
      if ((int)*plVar15 == 0) {
        (**(code **)*local_40)(local_40);
        LOCK();
        piVar1 = (int *)((longlong)local_40 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*local_40 + 8))(local_40);
        }
      }
    }
    plVar4 = local_58;
    plVar14 = plStack_60;
    plVar15 = local_68;
    memset(local_58,0,0x58);
    local_74 = FUN_1800094a0(param_1,(ulonglong)param_6);
    DVar6 = GetCurrentProcessId();
    _snprintf_s((char *)((longlong)plVar4 + 0x14),0x3f,0xffffffffffffffff,"{%d-%d-%d-%s-%d}",
                CONCAT44(uVar16,param_2),param_6,CONCAT44(uVar17,DVar6),param_1 + 0xc,local_74);
    cVar5 = HSlog_ShouldLog(1);
    if (cVar5 != '\0') {
      puVar9 = FUN_180001310();
      if (0xf < (ulonglong)puVar9[3]) {
        puVar9 = (undefined8 *)*puVar9;
      }
      HSlog_LogPrint(1,"[%s] [ModuId:%d] [%s@%d] Map view of file, name=%s",puVar9,param_2,
                     "CModuleSharedMemoryBase::AllocateSharedMemory",0x39,
                     (char *)((longlong)plVar4 + 0x14));
    }
  }
  else {
    plVar14 = *(longlong **)(*(longlong *)*plVar7 + 0x18);
    plVar15 = *(longlong **)(*(longlong *)*plVar7 + 0x10);
    if (plVar14 != (longlong *)0x0) {
      LOCK();
      *(int *)(plVar14 + 1) = (int)plVar14[1] + 1;
      UNLOCK();
    }
    local_68 = plVar15;
    plStack_60 = plVar14;
    FUN_1800096f0(plVar7);
  }
LAB_1800081ff:
  uVar2 = *(uint *)(plVar15 + 2);
  if ((ulonglong)uVar2 < param_3 + 0x80U) {
    if (uVar2 != 0) {
      FUN_1800090f0((ulonglong)uVar2,param_2,plVar15);
    }
    *(int *)(plVar15 + 2) = (int)param_3;
    uVar10 = FUN_180008d90(param_1,param_2,plVar15);
    if ((int)uVar10 != 0) {
      FUN_1800090f0(param_1,param_2,plVar15);
      plVar7 = plStack_60;
      if (plVar14 != (longlong *)0x0) {
        LOCK();
        plVar14 = plVar14 + 1;
        lVar12 = *plVar14;
        *(int *)plVar14 = (int)*plVar14 + -1;
        UNLOCK();
        if ((int)lVar12 == 1) {
          (**(code **)*plStack_60)(plStack_60);
          LOCK();
          piVar1 = (int *)((longlong)plVar7 + 0xc);
          iVar13 = *piVar1;
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (iVar13 == 1) {
            (**(code **)(*plStack_60 + 8))();
          }
        }
      }
      return 0xe0000015;
    }
    plVar4 = (longlong *)*plVar15;
    *plVar4 = param_3;
    *(undefined2 *)((longlong)plVar4 + 0xe) = 0x80;
    *(undefined1 *)(plVar4 + 1) = 0;
    memcpy_s(plVar4 + 2,0x40,(void *)((longlong)plVar15 + 0x14),0x40);
  }
  plVar4 = (longlong *)*plVar15;
  *plVar4 = param_3;
  *(undefined4 *)(plVar4 + 10) = 0;
  *param_4 = *plVar15 + 0x80;
  *param_5 = (longlong)plVar15 + 0x14;
  puVar9 = (undefined8 *)*plVar7;
  lVar11 = FUN_18000ab00((longlong)plVar15 + 0x14,puVar9,(undefined8 *)puVar9[1],&local_68);
  lVar12 = plVar7[1];
  if (lVar12 == 0x7fffffffffffffe) {
    std::_Xlength_error("list<T> too long");
    lVar12 = extraout_RAX;
  }
  plVar15 = plStack_60;
  plVar7[1] = lVar12 + 1;
  puVar9[1] = lVar11;
  **(longlong **)(lVar11 + 8) = lVar11;
  if (plVar14 != (longlong *)0x0) {
    LOCK();
    plVar14 = plVar14 + 1;
    lVar12 = *plVar14;
    *(int *)plVar14 = (int)*plVar14 + -1;
    UNLOCK();
    if ((int)lVar12 == 1) {
      (**(code **)*plStack_60)(plStack_60);
      LOCK();
      piVar1 = (int *)((longlong)plVar15 + 0xc);
      iVar13 = *piVar1;
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (iVar13 == 1) {
        (**(code **)(*plStack_60 + 8))();
      }
    }
  }
  return 0;
}

