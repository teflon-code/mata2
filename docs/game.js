"use strict";

const canvas = document.getElementById("game");
const ctx = canvas.getContext("2d");
const startPanel = document.getElementById("start");

const roads = [
  {name:"Uhuru Highway",lanes:4,points:[[36.81700,-1.28500],[36.81675,-1.28620],[36.81635,-1.28755],[36.81595,-1.28920],[36.81555,-1.29110],[36.81515,-1.29320]]},
  {name:"Moi Avenue",lanes:2,points:[[36.82210,-1.28315],[36.82205,-1.28420],[36.82195,-1.28535],[36.82180,-1.28655],[36.82165,-1.28785],[36.82145,-1.28915]]},
  {name:"Kenyatta Avenue",lanes:2,points:[[36.81480,-1.28420],[36.81615,-1.28435],[36.81755,-1.28455],[36.81895,-1.28480],[36.82035,-1.28505],[36.82165,-1.28530]]},
  {name:"Haile Selassie Avenue",lanes:4,points:[[36.81460,-1.28990],[36.81635,-1.28955],[36.81815,-1.28935],[36.81990,-1.28920],[36.82170,-1.28910],[36.82320,-1.28900]]},
  {name:"University Way",lanes:2,points:[[36.81380,-1.28735],[36.81545,-1.28715],[36.81715,-1.28695],[36.81890,-1.28675],[36.82060,-1.28660],[36.82200,-1.28650]]},
  {name:"Muindi Mbingu Street",lanes:2,points:[[36.81810,-1.28345],[36.81800,-1.28465],[36.81790,-1.28585],[36.81770,-1.28715],[36.81755,-1.28845],[36.81740,-1.28960]]},
  {name:"Mama Ngina Street",lanes:2,points:[[36.81610,-1.28240],[36.81730,-1.28255],[36.81855,-1.28275],[36.81985,-1.28295],[36.82110,-1.28310]]},
  {name:"Banda Street",lanes:2,points:[[36.81955,-1.28345],[36.81945,-1.28465],[36.81935,-1.28585],[36.81920,-1.28710],[36.81905,-1.28835]]},
  {name:"Accra Road",lanes:2,points:[[36.82320,-1.29000],[36.82215,-1.28975],[36.82105,-1.28945],[36.81995,-1.28920],[36.81880,-1.28895]]}
];

const lon0 = 36.8188;
const lat0 = -1.2867;
const scale = 7900;
function point(lonLat) {
  return {
    x: (lonLat[0] - lon0) * scale,
    y: -(lonLat[1] - lat0) * scale
  };
}

const checkpointCoords = [
  [36.81595,-1.28920],[36.82165,-1.28910],[36.82165,-1.28530],
  [36.81580,-1.28435],[36.81555,-1.29110]
];
const checkpoints = checkpointCoords.map(point);

const car = {x:-8,y:23,angle:Math.PI*.92,speed:0};
const keys = {up:false,down:false,left:false,right:false,brake:false};
let running = false;
let checkpointIndex = 0;
let startTime = 0;
let elapsed = 0;
let last = performance.now();
let audio = null;

function resize() {
  const dpr = Math.min(window.devicePixelRatio || 1, 2);
  canvas.width = innerWidth * dpr;
  canvas.height = innerHeight * dpr;
  canvas.style.width = innerWidth + "px";
  canvas.style.height = innerHeight + "px";
  ctx.setTransform(dpr,0,0,dpr,0,0);
}
addEventListener("resize", resize);
resize();

function reset() {
  car.x = point([36.81635,-1.28755]).x;
  car.y = point([36.81635,-1.28755]).y;
  car.angle = Math.PI*.92;
  car.speed = 0;
  checkpointIndex = 0;
  startTime = performance.now();
  elapsed = 0;
  document.getElementById("objective").textContent = "Checkpoint 1 / 5";
  document.getElementById("complete").classList.add("hidden");
}

function start() {
  reset();
  running = true;
  startPanel.classList.add("hidden");
  if (!audio) {
    audio = new (window.AudioContext || window.webkitAudioContext)();
  }
  if (audio.state === "suspended") audio.resume();
}
document.getElementById("startButton").addEventListener("click", start);
document.getElementById("againButton").addEventListener("click", start);

const keyMap = {ArrowUp:"up",KeyW:"up",ArrowDown:"down",KeyS:"down",ArrowLeft:"left",KeyA:"left",ArrowRight:"right",KeyD:"right",Space:"brake"};
addEventListener("keydown", e => { if (keyMap[e.code]) { keys[keyMap[e.code]]=true; e.preventDefault(); } });
addEventListener("keyup", e => { if (keyMap[e.code]) { keys[keyMap[e.code]]=false; e.preventDefault(); } });

for (const button of document.querySelectorAll("#controls button")) {
  const name = button.dataset.key;
  const set = value => { keys[name]=value; button.classList.toggle("pressed",value); };
  button.addEventListener("pointerdown", e => { e.preventDefault(); button.setPointerCapture(e.pointerId); set(true); });
  button.addEventListener("pointerup", () => set(false));
  button.addEventListener("pointercancel", () => set(false));
}

function update(dt) {
  if (!running) return;
  elapsed = (performance.now()-startTime)/1000;

  const throttle = keys.up ? 1 : keys.down ? -0.55 : 0;
  if (throttle) car.speed += throttle * 56 * dt;
  if (keys.brake) car.speed *= Math.pow(.05, dt);
  car.speed *= Math.pow(.992, dt*60);
  car.speed = Math.max(-18, Math.min(42, car.speed));

  if (Math.abs(car.speed) > 1) {
    const steer = (keys.left?-1:0)+(keys.right?1:0);
    car.angle += steer * 1.7 * dt * Math.min(1,Math.abs(car.speed)/18) * (car.speed>=0?1:-1);
  }
  car.x += Math.sin(car.angle)*car.speed*dt;
  car.y -= Math.cos(car.angle)*car.speed*dt;

  const target = checkpoints[checkpointIndex];
  if (target) {
    const dx=car.x-target.x, dy=car.y-target.y;
    if (Math.sqrt(dx*dx+dy*dy)<18) {
      checkpointIndex++;
      if (checkpointIndex>=checkpoints.length) {
        running=false;
        document.getElementById("finishTime").textContent="Finish time "+formatTime(elapsed);
        document.getElementById("complete").classList.remove("hidden");
      } else {
        document.getElementById("objective").textContent=`Checkpoint ${checkpointIndex+1} / ${checkpoints.length}`;
      }
    }
  }
  document.getElementById("speed").textContent=Math.round(Math.abs(car.speed)*3.6)+" km/h";
}

function formatTime(seconds) {
  const m=Math.floor(seconds/60), s=Math.floor(seconds%60);
  return String(m).padStart(2,"0")+":"+String(s).padStart(2,"0");
}

function worldToScreen(p,cx,cy) {
  return {x:innerWidth*.5+(p.x-cx),y:innerHeight*.58+(p.y-cy)};
}

function drawRoadLine(p1,p2,width) {
  ctx.beginPath();
  ctx.moveTo(p1.x,p1.y);
  ctx.lineTo(p2.x,p2.y);
  ctx.strokeStyle="#282c2c";
  ctx.lineWidth=width;
  ctx.lineCap="round";
  ctx.stroke();
  ctx.strokeStyle="rgba(255,215,90,.7)";
  ctx.lineWidth=1.2;
  ctx.setLineDash([10,14]);
  ctx.stroke();
  ctx.setLineDash([]);
}

function draw() {
  const camera = {x:car.x,y:car.y};
  ctx.fillStyle="#74a95f";
  ctx.fillRect(0,0,innerWidth,innerHeight);

  for (let gx=-innerWidth/2;gx<innerWidth*1.5;gx+=42) {
    for (let gy=-innerHeight/2;gy<innerHeight*1.5;gy+=42) {
      if ((Math.floor(gx/42)+Math.floor(gy/42))%3===0) {
        ctx.fillStyle="rgba(70,125,78,.22)";
        ctx.fillRect(gx+(camera.x%42),gy+(camera.y%42),28,28);
      }
    }
  }

  for (const road of roads) {
    const pts=road.points.map(point);
    for (let i=0;i<pts.length-1;i++) {
      const a=worldToScreen(pts[i],camera.x,camera.y);
      const b=worldToScreen(pts[i+1],camera.x,camera.y);
      drawRoadLine(a,b,Math.max(7,road.lanes*3.6));
    }
  }

  for (let i=0;i<checkpoints.length;i++) {
    if (i<checkpointIndex) continue;
    const p=worldToScreen(checkpoints[i],camera.x,camera.y);
    ctx.beginPath();
    ctx.arc(p.x,p.y,i===checkpointIndex?14:11,0,Math.PI*2);
    ctx.fillStyle=i===checkpointIndex?"rgba(239,140,34,.75)":"rgba(75,180,94,.55)";
    ctx.fill();
    ctx.strokeStyle="#fff";
    ctx.lineWidth=3;
    ctx.stroke();
  }

  const cx=innerWidth*.5, cy=innerHeight*.58;
  ctx.save();
  ctx.translate(cx,cy);
  ctx.rotate(-car.angle);
  ctx.fillStyle="#168849";
  ctx.fillRect(-9,-15,18,30);
  ctx.fillStyle="#ef8c22";
  ctx.fillRect(-9,-3,18,3);
  ctx.fillStyle="#18333a";
  ctx.fillRect(-8,-10,16,9);
  ctx.fillStyle="#f7d35c";
  ctx.fillRect(-6,12,3,2);
  ctx.fillRect(3,12,3,2);
  ctx.restore();

  ctx.fillStyle="rgba(9,24,18,.18)";
  ctx.fillRect(0,0,innerWidth,innerHeight);
}

function loop(now) {
  const dt=Math.min(.033,(now-last)/1000);
  last=now;
  update(dt);
  draw();
  requestAnimationFrame(loop);
}
reset();
requestAnimationFrame(loop);
