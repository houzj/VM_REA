
undefined8 FUN_180008380(longlong param_1,int param_2,char *param_3)

{
  longlong *plVar1;
  int *piVar2;
  char cVar3;
  longlong *plVar4;
  code *pcVar5;
  char cVar6;
  int iVar7;
  undefined8 *puVar8;
  longlong *plVar9;
  longlong *plVar10;
  char *pcVar11;
  longlong lVar12;
  int local_res18;
  int local_res20 [2];
  undefined8 in_stack_ffffffffffffffb8;
  undefined4 uVar13;
  undefined8 uVar14;
  longlong local_30;
  undefined8 *local_28;
  longlong *plStack_20;
  
  uVar13 = (undefined4)((ulonglong)in_stack_ffffffffffffffb8 >> 0x20);
  uVar14 = 0xfffffffffffffffe;
  if (param_3 != (char *)0x0) {
    iVar7 = sscanf_s(param_3,"{%d-%d-*}");
    if (iVar7 != 2) {
      cVar6 = HSlog_ShouldLog(4);
      if (cVar6 != '\0') {
        if ((DAT_180019b48 & 1) == 0) {
          DAT_180019b48 = DAT_180019b48 | 1;
          FUN_180001350();
          atexit(FUN_1800111b0);
        }
        puVar8 = &DAT_180019b28;
        if (0xf < DAT_180019b40) {
          puVar8 = DAT_180019b28;
        }
        HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] %s parse fail",puVar8,0xffffffff,
                       "CModuleSharedMemoryBase::ParseSharedName",0x17c,param_3);
      }
      return 0xe0000018;
    }
    if (param_2 == local_res18) {
      local_28 = (undefined8 *)0x0;
      plStack_20 = (longlong *)0x0;
      plVar9 = FUN_180009660((longlong *)(param_1 + 0x70),local_res20);
      plVar10 = (longlong *)*plVar9;
      pcVar11 = (char *)(*(longlong *)(plVar10[1] + 0x10) + 0x14);
      lVar12 = (longlong)param_3 - (longlong)pcVar11;
      do {
        cVar6 = *pcVar11;
        cVar3 = pcVar11[lVar12];
        if (cVar6 != cVar3) break;
        pcVar11 = pcVar11 + 1;
      } while (cVar3 != '\0');
      if (cVar6 == cVar3) {
        FUN_1800098d0(&local_28,(undefined8 *)(plVar10[1] + 0x10));
        plVar10 = *(longlong **)(*plVar9 + 8);
        *(longlong *)plVar10[1] = *plVar10;
        *(longlong *)(*plVar10 + 8) = plVar10[1];
        plVar9[1] = plVar9[1] + -1;
        plVar4 = (longlong *)plVar10[3];
        if (plVar4 != (longlong *)0x0) {
          LOCK();
          plVar1 = plVar4 + 1;
          *(int *)plVar1 = (int)*plVar1 + -1;
          UNLOCK();
          if ((int)*plVar1 == 0) {
            (**(code **)*plVar4)(plVar4);
            LOCK();
            piVar2 = (int *)((longlong)plVar4 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 == 0) {
              (**(code **)(*plVar4 + 8))(plVar4);
            }
          }
        }
        operator_delete(plVar10);
        puVar8 = local_28;
LAB_1800085df:
        if (puVar8 != (undefined8 *)0x0) {
          FUN_1800090f0(plVar10,0xffffffff,puVar8);
          puVar8 = *(undefined8 **)*plVar9;
          lVar12 = FUN_18000ab00(plVar10,puVar8,(undefined8 *)puVar8[1],&local_28);
          if (plVar9[1] == 0x7fffffffffffffe) {
            std::_Xlength_error("list<T> too long");
            pcVar5 = (code *)swi(3);
            uVar14 = (*pcVar5)();
            return uVar14;
          }
          plVar9[1] = plVar9[1] + 1;
          puVar8[1] = lVar12;
          **(longlong **)(lVar12 + 8) = lVar12;
        }
      }
      else {
        for (plVar4 = (longlong *)*plVar10; plVar4 != plVar10; plVar4 = (longlong *)*plVar4) {
          pcVar11 = (char *)(plVar4[2] + 0x14);
          lVar12 = (longlong)param_3 - (longlong)pcVar11;
          do {
            cVar6 = *pcVar11;
            cVar3 = pcVar11[lVar12];
            if (cVar6 != cVar3) break;
            pcVar11 = pcVar11 + 1;
          } while (cVar3 != '\0');
          if (cVar6 == cVar3) {
            plStack_20 = (longlong *)plVar4[3];
            puVar8 = (undefined8 *)plVar4[2];
            if (plStack_20 != (longlong *)0x0) {
              LOCK();
              *(int *)(plStack_20 + 1) = (int)plStack_20[1] + 1;
              UNLOCK();
            }
            plVar10 = plVar9;
            local_28 = puVar8;
            FUN_180009760((longlong)plVar9,&local_30,plVar4);
            goto LAB_1800085df;
          }
        }
      }
      plVar10 = plStack_20;
      if (plStack_20 != (longlong *)0x0) {
        LOCK();
        plVar9 = plStack_20 + 1;
        *(int *)plVar9 = (int)*plVar9 + -1;
        UNLOCK();
        if ((int)*plVar9 == 0) {
          (**(code **)*plStack_20)(plStack_20);
          LOCK();
          piVar2 = (int *)((longlong)plVar10 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_20 + 8))();
          }
        }
      }
      return 0;
    }
    cVar6 = HSlog_ShouldLog(4);
    if (cVar6 != '\0') {
      puVar8 = FUN_180001310();
      if (0xf < (ulonglong)puVar8[3]) {
        puVar8 = (undefined8 *)*puVar8;
      }
      HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] Module[%d] want release sharedmemory[%s]",puVar8,
                     param_2,"CModuleSharedMemoryBase::ReleaseSharedMemory",0x80,
                     CONCAT44(uVar13,param_2),param_3,uVar14);
    }
  }
  return 0xe0000001;
}

