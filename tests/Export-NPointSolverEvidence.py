"""Generate controlled no-device original SDK scenarios and retain the complete results."""
import hashlib,json,math,os,pathlib,subprocess,sys
root=pathlib.Path(__file__).resolve().parents[1]
out=root/'analysis/full-audit/n-point-solver';out.mkdir(parents=True,exist_ok=True)
cases=[]
def add(name,points,freedom=2,weight=1,mode=1,rotation=[],origin=[0,0],weightFactor=20,distThreshold=30,sampleRatio=60):
    cases.append(dict(name=name,points=points,freedom=freedom,weight=weight,mode=mode,rotation=rotation,origin=origin,weightFactor=weightFactor,distThreshold=distThreshold,sampleRatio=sampleRatio))
grid=[(x,y) for y in [0,10,20] for x in [0,10,20]]
for kind in ['scale','affine','perspective','noise','outlier','mirror']:
    p=[]
    for i,(x,y) in enumerate(grid):
        u,v=(2*x+10,2*y-5)
        if kind=='affine':u,v=2*x+.3*y+10,-.2*x+1.5*y-5
        if kind=='perspective':z=.003*x+.002*y+1;u,v=(2*x+.3*y+10)/z,(-.2*x+1.5*y-5)/z
        if kind=='noise':u+=(i%3-1)*.13;v+=((i*7)%5-2)*.21
        if kind=='outlier' and i==8:u+=12;v-=9
        if kind=='mirror':u=-2*x+10
        p.append([x,y,u,v])
    for f in [1,2,3]:
        for w in [1,2,3]:
            for mode in [1,2,3]:add(f'{kind}-f{f}-w{w}-m{mode}',p,f,w,mode)
for n in range(0,10):
    for f in [1,2,3]:add(f'count{n}-f{f}',[[x,y,2*x+10,2*y-5] for x,y in grid[:n]],f)
for n in [3,4,9]:
    for f in [1,2,3]:add(f'line{n}-f{f}',[[i*10,0,i*20+10,-5] for i in range(n)],f)
add('state-rotation-three',[[x,y,2*x+10,2*y-5] for x,y in grid],weight=3,rotation=[[0,30,30,15,109],[10,30,30,15,110],[20,30,30,15,111]])
for mode in [1,2,3]:
    for center in [(0,0),(30,40)]:
        for angles in [[0],[0,30],[0,30,60],[0,-30,-60],[0,90,180,270]]:
            rot=[]
            for a in angles:
                t=math.radians(a);x=center[0]+10*math.cos(t);y=center[1]+10*math.sin(t)
                rot.append([x,y,0,0,a])
            add(f'rotate-m{mode}-c{center[0]}-a{len(angles)}-{angles[-1]}',[[x,y,2*x+10,2*y-5] for x,y in grid],rotation=rot,mode=mode,origin=[center[0]+10,center[1]])
for kind in ['noise','outlier','perspective']:
    points=next(c['points'] for c in cases if c['name']==kind+'-f2-w1-m1')
    for dt in [1,10,30,100,1000]:
        for sr in [20,60,100]:add(f'parameters-{kind}-d{dt}-s{sr}',points,distThreshold=dt,sampleRatio=sr)
    for freedom in [2,3]:
        for w in [1,2,3]:
            for wf in [1,5,20,100]:add(f'weight-{kind}-h{freedom}-w{w}-f{wf}',points,freedom=freedom,weight=w,weightFactor=wf)
for mode in [1,2,3]:
    for sign in [-1,1]:
        for origin in [[0,0],[40,40]]:
            rot=[]
            for a in [0,30,60]:
                t=math.radians(sign*a);rot.append([30+10*math.cos(t),40+10*math.sin(t),0,0,a])
            add(f'rot-extra-m{mode}-sign{sign}-o{origin[0]}',[[x,y,2*x+10,2*y-5] for x,y in grid],rotation=rot,mode=mode,origin=origin)
for mode in [1,2,3]:
    for origin in [[10,20],[-10,5]]:add(f'origin-m{mode}-x{origin[0]}',[[x,y,2*x+10,2*y-5] for x,y in grid],mode=mode,origin=origin)
for delta in [0.25,0.5,1,2,3,4,6,8,10,12]:
    points=[[x,y,2*x+10,2*y-5] for x,y in grid];points[-1][2]+=delta
    add(f'error-threshold-{delta}',points)
for mode in [1,2,3]:
    for delta in [0,5]:
        for noise in [0,.5]:
            rot=[]
            for i,a in enumerate([0,30,60,90]):
                t=math.radians(a);rot.append([30+10*math.cos(t)+(delta*i)/2,40+10*math.sin(t)+noise*(i%2),delta*i,0,a])
            add(f'rotation-translation-m{mode}-d{delta}-n{noise}',[[x,y,2*x+10,2*y-5] for x,y in grid],rotation=rot,mode=mode,origin=[40,40])
for mode in [1,2,3]:
    for delta in [5,20]:
        rot=[]
        for i,a in enumerate([0,30,60,90]):
            t=math.radians(a);rot.append([30+10*math.cos(t),40+10*math.sin(t),10+delta*i,15,a])
        add(f'rotation-world-m{mode}-d{delta}',[[x,y,2*x+10,2*y-5] for x,y in grid],rotation=rot,mode=mode)
data=[]
for mode in [1,2,3]:add(f'rotation-identical-m{mode}',[[x,y,2*x+10,2*y-5] for x,y in grid],mode=mode,rotation=[[59.210609436,63.894184113,0,0,a] for a in [0,30,60]])
for parameter in ['weightFactor','distThreshold','sampleRatio']:
    for value in [0,101]:add(f'invalid-{parameter}-{value}',[[x,y,2*x+10,2*y-5] for x,y in grid],**{parameter:value})
for c in cases:
    data.append(' '.join(map(str,[c['name'],c['mode'],c['freedom'],c['weight'],len(c['points']),len(c['rotation']),*c['origin'],c['weightFactor'],c['distThreshold'],c['sampleRatio']])))
    data += [' '.join(map(str,p)) for p in c['points']+c['rotation']]
(out/'cases.txt').write_text('\n'.join(data)+'\n')
runtime=pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64')
assert hashlib.sha256((runtime/'MVDNPointCalibCpp.dll').read_bytes()).hexdigest()=='74b65daacb1fc0b48abe39ec0603dc4fc6e70c0a910680787ab318fdc7d0fa17'
assert hashlib.sha256((runtime/'MVD_Algorithm.dll').read_bytes()).hexdigest()=='85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549'
env=os.environ.copy();env['PATH']=str(runtime)+';'+env['PATH']
if '--reuse' not in sys.argv:
    with (out/'original.jsonl').open('w') as stream:
        proc=subprocess.run([str(out/'solver.exe'),str(runtime/'MVDNPointCalibCpp.dll'),str(out/'cases.txt')],env=env,stdout=stream,stderr=subprocess.PIPE,text=True,timeout=300,check=True)
    (out/'probe-stderr.txt').write_text(proc.stderr)
rows=[json.loads(line) for line in (out/'original.jsonl').read_text().splitlines() if line.startswith('{') and line.endswith('}')]
assert len(rows)==len(cases),(len(rows),len(cases))
report=dict(scope='Owned-process original SDK algorithm; synthetic numerical point sets; no devices',binarySha256=hashlib.sha256((runtime/'MVDNPointCalibCpp.dll').read_bytes()).hexdigest(),probeSha256=hashlib.sha256((root/'tests/NativeEvidence/OriginalNPointSolverProbe.cpp').read_bytes()).hexdigest(),cases=[dict(**c,result=r) for c,r in zip(cases,rows)])
(root/'docs/plugins/n-point-solver-evidence.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(f'{len(rows)} original algorithm runs retained')
