import { __assign } from 'tslib';
export class DiagramCanVas {
	//******biến dùng chung********* */
	//các thuộc tính hằng số để cài đặt cho canvas
	canvas: any; //canvas
	canvas_width: number = 60;//chiều dài canvas
	canvas_height: number = 40;//chiều cao canvas
	widthheadarrow = 5;//chiều dài của mũi arrow
	LineWidth = 1;//độ dày của đường line
	GridSize = 20;//độ rộng của Gird tọa độ
	widthRect = 5;//chiều dài hỉnh chữ nhật
	heightRect = 3;//chiều cao hình chữ nhật
	arrow_w = 2;//chiều dài đơn vị của line
	arrow_h = 4;//chiều cao đơn vị của line
	color_back="#FF0000";
	//version 2
	fontSize = 12;
	//biến thay đổi trên canvas
	arrNode: any[] = [];
	arrArrow: any[] = [];
	panX = 0; //tọa độ dịch chuyển trục x
	panY = 0;//tọa dộ dịch chuyển trục y
	ctx: any; //context của canvas
	scaleFactor = 1; //độ scale của canvas
	mouseDown = false; // dùng cho sự kiện dịch chuyển canvas khi chọn Space + rê chuột trên canvas
	space = false; //xác định có nhấn phím space hay không(Nếu nhấn space thì mới di chuyển trên canvas được)
	offsetX = 0;
	offsetY = 0;
	tooltip: any;
	__options = null;
	selectedNode: any;
	isHover = true;
	// matrixPos = [1, 0, 0, 1, 0, 0];
	//set param for event scale
	// lazy programmers globals
	scale = 1;
	wx = 0; // world zoom origin
	wy = 0;
	sx = 0; // mouse screen pos
	sy = 0;

	mouse = {
		x: 0,// pixel pos of mouse
		y: 0,
		rx: 0, // mouse real (world) pos
		ry: 0,
		button: 0,
		bounds: {
			top: 0,
			left: 0
		}
	};


	//*******biến cục bộ************
	// private lastX = 0; //tọa độ của điểm cuối cùng khi rê chuột trục x
	// private lastY = 0; //tọa độ của điểm cuối cùng khi rê chuột trục y
	private rects: any[] = []; // mảng tọa độ các node
	private lines: any[] = [];//mảng tọa độ các lines

	//parameter version 1
	private arrNode_Arrow: any[] = []; //mảng tổng hợp arrow trên từng node
	private level_left = 1; //level cộng dồn để cách nhau bên trái
	private level_right = 1;//level cộng dồn để cách nhau bên phải

	//parameter Version 2
	private arr_Level: any[] = [];
	private arr_arrowFake: any[] = [];
	private isLeft = true;

	//
	constructor(el, nodes, arrows, tooltip, options) {
		this.arrNode = nodes;
		this.arrArrow = arrows;
		this.canvas = el;
		this.ctx = this.canvas.getContext("2d");
		this.scaleFactor = 1;
		this.panX = 0;
		this.panY = 0;
		this.tooltip = tooltip;
		this.__options = options;
		/************Sort before draw************ */
		this.arrArrow = this.arrArrow.sort((a, b) => {
			return a.Start - b.Start;
		});

	}
	//************region Set Position********** */
	zoomed(number) { // just scale
		return Math.floor(number * this.scale);
	}
	// converts from world coord to screen pixel coord
	zoomedX(number) { // scale & origin X
		return Math.floor((number - this.wx) * this.scale + this.sx);
	}

	zoomedY(number) { // scale & origin Y
		return Math.floor((number - this.wy) * this.scale + this.sy);
	}
	resetCanvas() {
		this.scale = 1;
		this.wx = 0; // world zoom origin
		this.wy = 0;
		this.sx = 0; // mouse screen pos
		this.sy = 0;

		this.mouse = {
			x: 0,// pixel pos of mouse
			y: 0,
			rx: 0, // mouse real (world) pos
			ry: 0,
			button: 0,
			bounds: {
				top: 0,
				left: 0
			}
		};
	}
	// Inverse does the reverse of a calculation. Like (3 - 1) * 5 = 10   the inverse is 10 * (1/5) + 1 = 3
	// multiply become 1 over ie *5 becomes * 1/5  (or just /5)
	// Adds become subtracts and subtract become add.
	// and what is first become last and the other way round.

	// inverse function converts from screen pixel coord to world coord
	zoomedX_INV(number) { // scale & origin INV
		return Math.floor((number - this.sx) * (1 / this.scale) + this.wx);
		// or return Math.floor((number - sx) / scale + wx);
	}

	zoomedY_INV(number) { // scale & origin INV
		return Math.floor((number - this.sy) * (1 / this.scale) + this.wy);
		// or return Math.floor((number - sy) / scale + wy);
	}
	move(event) { // mouse move event
		if (event.type == "mouseover") {
			this.isHover = true;
		}
		if (event.type == "mouseout") {
			this.isHover = false;
		}
		if (event.type === "mousedown") {
			this.mouse.button = 1;
		}
		else if (event.type === "mouseup" || event.type === "mouseout") {
			this.mouse.button = 0;
		}
		this.mouse.bounds = this.canvas.getBoundingClientRect();
		this.mouse.x = event.clientX - this.mouse.bounds.left;
		this.mouse.y = event.clientY - this.mouse.bounds.top;
		var xx = this.mouse.rx; // get last real world pos of mouse
		var yy = this.mouse.ry;

		this.mouse.rx = this.zoomedX_INV(this.mouse.x); // get the mouse real world pos via inverse scale and translate
		this.mouse.ry = this.zoomedY_INV(this.mouse.y);
		if (this.mouse.button === 1 && this.space) { // is mouse button down 
			this.wx -= this.mouse.rx - xx; // move the world origin by the distance 
			// moved in world coords
			this.wy -= this.mouse.ry - yy;
			// recaculate mouse world 
			this.mouse.rx = this.zoomedX_INV(this.mouse.x);
			this.mouse.ry = this.zoomedY_INV(this.mouse.y);
			if (this.__options && this.__options.onMoved && typeof (this.__options.onMoved) == "function") {
				this.__options.onMoved(event);
			}
		}
		this.Draw();
		this.tooltip.style.display = "none";
		this.rects.forEach(rect => {
			// let pos_rect = this.getNewPosition(rect.x, rect.y);           

			let x_limit_rect_start = rect.x * this.GridSize;// - this.panX* this.scaleFactor;
			let x_limit_rect_end = (rect.x + rect.w) * this.GridSize;// - this.panX* this.scaleFactor;
			let y_limit_rect_start = rect.y * this.GridSize;// - this.panY* this.scaleFactor;
			let y_limit_rect_end = (rect.y + rect.h) * this.GridSize;// - this.panY* this.scaleFactor;  

			if (this.mouse.rx > x_limit_rect_start && this.mouse.rx < x_limit_rect_end && this.mouse.ry > y_limit_rect_start && this.mouse.ry < y_limit_rect_end) {
				if (rect.tip) {
					this.setPositionTooltip(this.tooltip, rect.tip, event.offsetX, event.offsetY);
				}
				if (this.mouse.button == 1) {
					let info = this.getcontentRelate(rect.ID);
					this.selectedNode = info;

					if (this.__options && this.__options.nodeClicked && typeof (this.__options.nodeClicked) == "function") {
						this.__options.nodeClicked(this.selectedNode, this.__options._this);
					}
				}
			}
		});

	}
	getcontentRelate(ID) {
		let node = this.getNodeByID(ID);
		var info = {
			ID: ID,
			Title: node.Title,
			data: node.data,
			arrows: [],
			arrows_inverse: []
		}
		this.arr_arrowFake.forEach(element => {
			let item_arr = {
				start_node: {},
				end_node: {},
				arrow: {}
			}
			if (element.Start == ID) {
				item_arr.start_node = node;
				item_arr.arrow = element;
				let nodeEnd = this.getNodeByID(element.End);
				if (nodeEnd) {
					item_arr.end_node = nodeEnd;
				}
				info.arrows.push(item_arr);
			}
			if (element.End == ID) {
				item_arr.end_node = node;
				item_arr.arrow = element;
				let nodeStart = this.getNodeByID(element.Start);
				if (nodeStart) {
					item_arr.start_node = nodeStart;
				}
				info.arrows_inverse.push(item_arr);
			}
		});
		return info;
	}
	trackWheel(e) {
		e.preventDefault(); // stop the page scrolling
		if (e.deltaY < 0) {
			this.scale = Math.min(5, this.scale * 1.1); // zoom in
		} else {
			this.scale = Math.max(0.1, this.scale * (1 / 1.1)); // zoom out is inverse of zoom in
		}
		this.wx = this.mouse.rx; // set world origin
		this.wy = this.mouse.ry;
		this.sx = this.mouse.x; // set screen origin
		this.sy = this.mouse.y;
		this.mouse.rx = this.zoomedX_INV(this.mouse.x); // recalc mouse world (real) pos
		this.mouse.ry = this.zoomedY_INV(this.mouse.y);
		this.Draw();
		if (this.__options && this.__options.onZoomed && typeof (this.__options.onZoomed) == "function") {
			this.__options.onZoomed(e);
		}
	}

	//********************************************* */
	Draw() {
		this.tooltip.style.display = "none";
		this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
		this.ctx.font = this.zoomed(this.fontSize) + "px Arial";
		this.ctx.beginPath();
		this.ctx.stroke();
		this.rects = [];
		this.lines = [];
		this.arr_arrowFake = [];

		//*********************Version 1
		// this.arrNode_Arrow = [];
		// this.level_left = 1;
		// this.level_right = 1;
		// this.CustomData();


		//*********************Version 2 */
		// this.arr_arrowFake = this.arrArrow.map(a => Object.assign({}, a));
		//Lọc lại những arrow đi đến node vô danh

		for (var i = 0; i < this.arrArrow.length; i++) {
			var arr = this.arrArrow[i];
			let nd_start = undefined;
			let nd_end = undefined;
			this.arrNode.forEach(node => {
				if (arr.Start == node.ID)
					nd_start = node;
				if (arr.End == node.ID)
					nd_end = node;
			});
			if (nd_start && nd_end) {//Hiện tại chưa vẽ node vòng chính nó  && nd_start.ID != nd_end.ID
				this.arr_arrowFake.push(arr);
			}
		}
		this.setLevelNode();
		this.drawLevelNode();
		//#region  ************************** MODEL CHART ***************************** */
		//     var rects: any[] = [
		//   { x: 2, y: 16, w: 10, h: 6 },
		//   { x: 18, y: 2, w: 10, h: 6 },
		//   { x: 18, y: 16, w: 10, h: 6 },
		//   { x: 36, y: 28, w: 10, h: 6 }

		// ];

		// var lines: any[] = [
		//   {
		//     startPos: { x: 18, y: 5, d: true },
		//     lines: [-11, 11]
		//   },
		//   {
		//     startPos: { x: 23, y: 8, d: false },
		//     lines: [8]
		//   },
		//   {
		//     startPos: { x: 28, y: 6, d: true },
		//     lines: [8, 6, -10, 4]
		//   },
		//   {
		//     startPos: { x: 28, y: 4, d: true },
		//     lines: [14, 15, -14]
		//   },
		//   {
		//     startPos: { x: 23, y: 22, d: false },
		//     lines: [3, 18, 3]
		//   },
		//   {
		//     startPos: { x: 23, y: 22, d: false },
		//     lines: [3, 18, 3]
		//   }
		// ];
		//#endregion
		this.rects.forEach(d => {

			this.DrawRect(d.x * this.GridSize, d.y * this.GridSize, d.w * this.GridSize, d.h * this.GridSize, d.c, d.b);
		});
		//console.log("lines", this.lines);
		this.lines.forEach(d => {
			this.DrawLines(d.startPos, d.lines);
		});
		this.ctx.closePath();
		if (this.__options && this.__options.onDraw && typeof (this.__options.onDraw) == "function") {
			this.__options.onDraw();
		}
	}
	// ZoomPan() {
	//     this.canvas.width = this.canvas_width * this.GridSize;
	//     this.canvas.height = this.canvas_height * this.GridSize;
	//     this.matrixPos = [1, 0, 0, 1, 0, 0];
	//     this.matrixPos[0] *= this.scaleFactor;
	//     this.matrixPos[1] *= this.scaleFactor;
	//     this.matrixPos[2] *= this.scaleFactor;
	//     this.matrixPos[3] *= this.scaleFactor;
	//     this.matrixPos[4] += this.matrixPos[0] * this.panX + this.matrixPos[2] * this.panY;
	//     this.matrixPos[5] += this.matrixPos[1] * this.panX + this.matrixPos[3] * this.panY;
	//     this.ctx.translate(this.panX, this.panY);
	//     this.ctx.scale(this.scaleFactor, this.scaleFactor);
	// }
	DrawRect(x, y, w, h, c, b) {
		if (this.canvas.getContext) {
			if (this.ctx.fillStyle && b) {
				this.ctx.fillStyle = b;
			}
			else {
				this.ctx.fillStyle = "#FFFFFF00";
			}
			if (this.ctx.strokeStyle) {
				this.ctx.strokeStyle = c;
			}
			else {
				this.ctx.strokeStyle = "#000000";
			}
			this.ctx.lineWidth = this.LineWidth;
			this.ctx.strokeRect(this.zoomedX(x), this.zoomedY(y), this.zoomed(w), this.zoomed(h));
			this.ctx.fillRect(this.zoomedX(x), this.zoomedY(y), this.zoomed(w), this.zoomed(h));
			this.ctx.strokeStyle = "#000000";
			this.ctx.fillStyle = "#FFFFFF00";
		}
	}


	DrawLines(startPos: any, lines: number[]) {
		// console.log("startpost", startPos);
		if (this.canvas.getContext) {
			this.ctx.beginPath();
			this.ctx.lineWidth = this.LineWidth;
			if(startPos.IsComeBack){
				this.ctx.strokeStyle = this.color_back;
			}
			else{
				this.ctx.strokeStyle = "#000000";
			}
			// this.ctx.strokeStyle = "#333333";          
			// this.ctx.moveTo(startPos.x * this.GridSize, startPos.y * this.GridSize);
			this.ctx.moveTo(this.zoomedX(startPos.x * this.GridSize), this.zoomedY(startPos.y * this.GridSize));
			let d = startPos.d;
			let x = startPos.x, y = startPos.y;
			for (var i = 0; i < lines.length; i++) {
				var v = lines[i];
				if (d) {
					x += v;
				}
				else {
					y += v;
				}
				if (startPos.arrHead == true) {//trường hợp mũi tên đi ngược
					if (i == 0) {//thêm mũi tên ngay từ đầu
						let sign = 1;
						if (v < 0) {
							sign = -1;
						}
						if (d) {
							this.ctx.moveTo(this.zoomedX((startPos.x * this.GridSize) + this.widthheadarrow * sign), this.zoomedY((startPos.y * this.GridSize) - this.widthheadarrow));
							this.ctx.lineTo(this.zoomedX(startPos.x * this.GridSize), this.zoomedY(startPos.y * this.GridSize));
							this.ctx.lineTo(this.zoomedX((startPos.x * this.GridSize) + this.widthheadarrow * sign), this.zoomedY((startPos.y * this.GridSize) + this.widthheadarrow));
						}
						else {
							this.ctx.moveTo(this.zoomedX((startPos.x * this.GridSize) - this.widthheadarrow), this.zoomedY((startPos.y * this.GridSize) + this.widthheadarrow));
							this.ctx.lineTo(this.zoomedX(startPos.x * this.GridSize), this.zoomedY(startPos.y * this.GridSize));
							this.ctx.lineTo(this.zoomedX((startPos.x * this.GridSize) + this.widthheadarrow), this.zoomedY((startPos.y * this.GridSize) + this.widthheadarrow));
						}
						// this.ctx.moveTo(startPos.x * this.GridSize, startPos.y * this.GridSize);
						this.ctx.moveTo(this.zoomedX(startPos.x * this.GridSize), this.zoomedY(startPos.y * this.GridSize));
					}
					// this.ctx.lineTo(x * this.GridSize, y * this.GridSize);
					this.ctx.lineTo(this.zoomedX(x * this.GridSize), this.zoomedY(y * this.GridSize));
				}
				else {//trường hợp mũi tên di xuôi
					// this.ctx.lineTo(x * this.GridSize, y * this.GridSize);
					this.ctx.lineTo(this.zoomedX(x * this.GridSize), this.zoomedY(y * this.GridSize));
					if (i == lines.length - 1) {//thêm mũi tên cuối cùng
						let sign = 1;
						if (v < 0) {
							sign = -1;
						}
						if (d) {
							this.ctx.moveTo(this.zoomedX((x * this.GridSize) - this.widthheadarrow * sign), this.zoomedY((y * this.GridSize) - this.widthheadarrow));
							this.ctx.lineTo(this.zoomedX(x * this.GridSize), this.zoomedY(y * this.GridSize));
							this.ctx.lineTo(this.zoomedX((x * this.GridSize) - this.widthheadarrow * sign), this.zoomedY((y * this.GridSize) + this.widthheadarrow));
						}
						else {
							this.ctx.moveTo(this.zoomedX((x * this.GridSize) - this.widthheadarrow), this.zoomedY((y * this.GridSize) - this.widthheadarrow * sign));
							this.ctx.lineTo(this.zoomedX(x * this.GridSize), this.zoomedY(y * this.GridSize));
							this.ctx.lineTo(this.zoomedX((x * this.GridSize) + this.widthheadarrow), this.zoomedY((y * this.GridSize) - this.widthheadarrow * sign));
						}
					}
				}
				this.ctx.stroke();
				d = !d;
			}
			this.ctx.closePath();
		}		
	}

	wrapText(context, text, x, y, maxWidth) {
		context.fillStyle = "#000000";
		var words = text.split(' ');
		var line = '';
		var lineHeight = this.fontSize * 1.286;
		var padding_x = 0.2 * this.GridSize;
		var padding_y = lineHeight;
		x += padding_x;
		y += padding_y;
		maxWidth -= padding_x;
		maxWidth = this.zoomed(maxWidth);
		let maxheight = y + this.heightRect * this.GridSize - padding_y;
		for (var n = 0; n < words.length; n++) {
			var testLine = line + words[n] + ' ';
			var metrics = context.measureText(testLine);
			var testWidth = metrics.width;
			if (testWidth > maxWidth && n > 0) {
				if (y + lineHeight > maxheight) {
					line = line.substring(0, line.length - 3) + '...';
					context.fillText(line, this.zoomedX(x), this.zoomedY(y));
					return;
				}
				context.fillText(line, this.zoomedX(x), this.zoomedY(y));
				line = words[n] + ' ';
				y += lineHeight;
			}
			else {
				line = testLine;
			}
		}
		context.fillText(line, this.zoomedX(x), this.zoomedY(y));
	}
	setPositionTooltip(el, text, x, y) {
		el.innerText = text;
		el.style.display = "block";
		el.style.top = (y + this.zoomed(this.fontSize)) + "px";
		el.style.left = (x + this.zoomed(this.fontSize)) + "px";
	}
	//#region ******************VERSION 1****************************************
	CustomData() {

		//add node
		for (var i = 0; i < this.arrNode.length; i++) {//for node
			var node = this.arrNode[i];
			let point_node = {
				x: ((this.canvas.width / this.GridSize) - this.widthRect) / 2,
				y: (this.heightRect + this.arrow_h) * i + 2,
				w: this.widthRect,
				h: this.heightRect
			};
			let item_node_arrows = {
				ID: node.ID,
				Title: node.Title,
				Location: point_node,
				arr_arrow: [],
				Relative: [0, 0, 0, 0]
			};
			this.arrNode_Arrow.push(item_node_arrows);
		}
		//add line
		this.pushArrowToNode();
		//console.log("list item arrow node", this.arrNode_Arrow);
		//set data to lines and rects
		for (var i = 0; i < this.arrNode_Arrow.length; i++) {
			let item_node = this.arrNode_Arrow[i];
			this.wrapText(this.ctx, item_node.Title, (item_node.Location.x) * this.GridSize, (item_node.Location.y) * this.GridSize, this.widthRect * this.GridSize);
			// this.ctx.fillText(item_node.Title, (item_node.Location.x + 0.5) * this.GridSize, (item_node.Location.y + 1) * this.GridSize); //viết text
			this.rects.push(item_node.Location);
			if (item_node.arr_arrow && item_node.arr_arrow.length > 0) {
				for (var j = 0; j < item_node.arr_arrow.length; j++) {
					let el = item_node.arr_arrow[j];
					let line = this.setDataLine(el, item_node);
					this.lines.push(line);
				}
			}
		}
	}
	//Thêm các arrow cho node
	pushArrowToNode() {
		this.arrNode_Arrow.forEach(element => {
			let pos_start = 0;
			let pos_end = 0;
			this.arrArrow.forEach(arr => {
				let item_arrow = {
					ID: arr.ID,
					Start: arr.Start,
					End: arr.End,
					Level: (arr.End - arr.Start),
					Idx_Start: -1,
					Idx_End: -1,
					IsComeBack: arr.IsComeBack
				}
				if (arr.Start == element.ID) {//|| arr.End == element.ID
					let idx = this.getIndexRelativeNode(item_arrow.Level);
					if (idx >= 0) {
						if (arr.Start == element.ID) {
							pos_start = this.getStartEndIndexRelativeNode(arr.Start, idx, item_arrow.Level, 0);
							pos_end = this.getStartEndIndexRelativeNode(arr.End, idx, item_arrow.Level, 1);
							item_arrow.Idx_Start = pos_start;
							item_arrow.Idx_End = pos_end;
						}
					}
					element.arr_arrow.push(item_arrow);
				}
				// if (arr.Start == element.ID) {
				//     element.arr_arrow.push(item_arrow);
				// }
			});
		});
	}
	//lấy mảng các mặt của node
	getNumRelative(Id) {
		let rlt = [];
		this.arrNode_Arrow.forEach(element => {
			if (element.ID == Id) {
				rlt = element.Relative;
			}
		});
		return rlt;
	}
	//lấy vị trí start or end tại các mặt của node
	getStartEndIndexRelativeNode(Id, idxRelate, level, type) {
		if (type == 1) {//end{
			let idx = idxRelate;
			if (level == 1 || level == -1) {
				idx = (idxRelate == 3) ? 1 : 3;
			}
			idxRelate = idx;
		}
		let idx_num = -1;
		this.arrNode_Arrow.forEach(element => {
			if (element.ID == Id) {
				element.Relative[idxRelate]++;
				idx_num = element.Relative[idxRelate];
			}
		});
		return idx_num;
	}
	//lấy vị trí mặt của node thứ tự trái, dưới, phải, trên
	getIndexRelativeNode(level) {
		let pos = -1;
		if (level > 1) {
			pos = 0;
		}
		else {
			if (level == 1) {
				pos = 1;
			}
			else if (level == -1) {
				pos = 3;
			}
			else {
				pos = 2;
			}
		}
		return pos;
	}

	//set dữ liệu lên line (vẽ line): el là dữ liệu line, item_node: dữ liệu của node chứa line
	setDataLine(el, item_node) {
		let line = {};
		let startPos = {};
		let arr_lines = [];
		let sign = 1;
		if (el.Level < 0) {
			sign = -1;
		}
		let arr_relate_start = item_node.Relative;//mảng các mặt của node chứa line
		let idx_arr_start = this.getIndexRelativeNode(el.Level);//lấy vị trí mặt start của line
		let arr_relate_end = this.getNumRelative(el.End);//mảng các mặt của node chứa line
		let idx_arr_end = idx_arr_start; //mặc định mặt của start và end bằng nhau

		if (el.Level == 1 * sign) { //trường hợp đi lên đi xuống 1 chiều theo đường thẳng
			//chia vị trí để lấy vị trí từng line cho trường hợp nhiều line trên 1 mặt của node
			idx_arr_end = (idx_arr_start == 3) ? 1 : 3;//nếu là level 1 hoặc -1 tức là đường thẳng thì mặt start =1 => end =3
			let num_ln_facenode = arr_relate_start[idx_arr_start];//nếu là đường thẳng thì độ dài width sẽ bằng nhau cho cả start và end
			let unit_w = this.widthRect / 2;
			if (num_ln_facenode > 1) {
				unit_w = this.widthRect / (num_ln_facenode + 1);
			}
			//
			let y_lc = this.heightRect;
			if (sign < 0) {
				y_lc = 0;
			}
			startPos = { x: item_node.Location.x + (unit_w * el.Idx_Start), y: item_node.Location.y + y_lc, d: false };
			arr_lines = [this.arrow_h * sign];
		}
		else {//trường hợp đi ngang
			//chia vị trí để lấy vị trí từng line cho trường hợp nhiều line trên 1 mặt của node
			let num_ln_facenode_start = arr_relate_start[idx_arr_start];//số lượng line trên mặt start của node
			let num_ln_facenode_end = arr_relate_end[idx_arr_end];// số lượng line trên mặt end cua node end
			let unit_w_start = this.heightRect / 2;
			let unit_w_end = this.heightRect / 2;
			if (num_ln_facenode_start > 1) {
				unit_w_start = this.heightRect / (num_ln_facenode_start + 1);
			}
			if (num_ln_facenode_end > 1) {
				unit_w_end = this.heightRect / (num_ln_facenode_end + 1);
			}
			//
			let x_lc = 0;
			let level_sign = 0;
			if (el.Level > 1) {//đi trên xuống dưới
				x_lc = item_node.Location.x;
				level_sign = this.level_left;
				this.level_left++;
			}
			else {//đi dưới lên trên
				x_lc = item_node.Location.x + this.widthRect;
				level_sign = this.level_right;
				this.level_right++;
			}
			startPos = { x: x_lc, y: item_node.Location.y + (unit_w_start * el.Idx_Start), d: true };
			arr_lines = [-this.arrow_w * level_sign * sign, this.arrow_h * el.Level + (el.Level - 1) * this.heightRect + (this.heightRect - unit_w_start * el.Idx_Start) + unit_w_end * el.Idx_End, this.arrow_w * level_sign * sign];
		}
		//set data line
		line = {
			startPos: startPos,
			lines: arr_lines,			
		}
		return line;
	}
	//#endregion
	// handleMouseDown(event) {
	//     this.mouseDown = true;
	//     this.lastX = event.offsetX * this.scaleFactor;
	//     this.lastY = event.offsetY * this.scaleFactor;

	// }
	// handleMouseUp(event) {
	//     this.mouseDown = false;
	// }
	// handleMouseMove(event) {
	//     let X = event.offsetX * this.scaleFactor;
	//     let Y = event.offsetY * this.scaleFactor;
	//     if (this.space && this.mouseDown) {
	//         this.panX += (X - this.lastX) / this.scaleFactor;
	//         this.panY += (Y - this.lastY) / this.scaleFactor;
	//         this.lastX = X;
	//         this.lastY = Y;
	//     }
	//     this.ZoomPan();
	//     this.Draw();
	//     let pos = this.getNewPosition(event.offsetX, event.offsetY);
	//     this.tooltip.style.display = "none";
	//     //console.log("mouse x: (" + pos.x + ", " + pos.y + ")");
	//     //console.log("Event: (" + event.offsetX + ", " + event.offsetY + ")");
	//     //console.log("matrix", this.matrixPos)
	//     //console.log("scaleFactor", this.scaleFactor)
	//     this.rects.forEach(rect => {
	//         // let pos_rect = this.getNewPosition(rect.x, rect.y);           
	//         if (rect.tip) {
	//             let x_limit_rect_start = rect.x * this.GridSize;// - this.panX* this.scaleFactor;
	//             let x_limit_rect_end = (rect.x + rect.w) * this.GridSize;// - this.panX* this.scaleFactor;
	//             let y_limit_rect_start = rect.y * this.GridSize;// - this.panY* this.scaleFactor;
	//             let y_limit_rect_end = (rect.y + rect.h) * this.GridSize;// - this.panY* this.scaleFactor;  

	//             if (pos.x > x_limit_rect_start && pos.x < x_limit_rect_end && pos.y > y_limit_rect_start && pos.y < y_limit_rect_end) {
	//                 //console.log("mouse x: (" + pos.x + ", " + pos.y + ")");
	//                 //console.log("limit rect x: (" + x_limit_rect_start + ", " + x_limit_rect_end + ")");
	//                 //console.log("limit rect y: (" + y_limit_rect_start + ", " + y_limit_rect_end + ")");
	//                 //console.log("tip", rect.tip);
	//                 // this.setPositionTooltip(this.tooltip, rect.tip, event.offsetX, event.offsetY);
	//             }
	//         }
	//     });
	// }
	// getNewPosition(mouseX, mouseY) {
	//     let newX = mouseX * this.matrixPos[0] + mouseY * this.matrixPos[2] + this.matrixPos[4];
	//     let newY = mouseX * this.matrixPos[1] + mouseY * this.matrixPos[3] + this.matrixPos[5];
	//     return ({ x: newX, y: newY });
	// }

	// DrawToolTip(context, text, x, y) {
	//     var words = text.split(' ');
	//     var line = '';
	//     var line2 = '';
	//     var lineHeight = 12 * 1.286;
	//     var padding_x = 0.2 * this.GridSize;
	//     var padding_y = lineHeight;
	//     let top = Object.assign(y) + lineHeight;
	//     let left = Object.assign(x);
	//     x += padding_x;
	//     y = top + padding_y;
	//     let maxWidth = 7 * this.GridSize;
	//     let maxHeight = 2 * lineHeight;
	//     maxWidth -= padding_x;
	//     for (var n = 0; n < words.length; n++) {
	//         var testLine = line2 + words[n] + ' ';
	//         var metrics = context.measureText(testLine);
	//         var testWidth = metrics.width;
	//         if (testWidth > maxWidth && n > 0) {
	//             // context.fillText(line, x, y);
	//             line2 = words[n] + ' ';
	//             // y += lineHeight;
	//             maxHeight += lineHeight;
	//         }
	//         else {
	//             line2 = testLine;
	//         }
	//     }
	//     context.fillStyle = "#ffffff";
	//     context.strokeStyle = "#0e64f5";
	//     context.fillRect(left, top, maxWidth, maxHeight);
	//     context.strokeRect(left, top, maxWidth, maxHeight);
	//     context.strokeStyle = "#000000";
	//     context.fillStyle = "#000000";
	//     for (var n = 0; n < words.length; n++) {
	//         var testLine = line + words[n] + ' ';
	//         var metrics = context.measureText(testLine);
	//         var testWidth = metrics.width;
	//         if (testWidth > maxWidth && n > 0) {
	//             context.fillText(line, x, y);
	//             line = words[n] + ' ';
	//             y += lineHeight;
	//         }
	//         else {
	//             line = testLine;
	//         }
	//     }
	//     context.fillText(line, x, y);
	// }
	//************Handle Zoom*********** *//
	//    handleMouseWheel(event) {
	//     //console.log("event", event);
	//     let delta = event.deltaY;
	//     //console.log("delta", delta);
	//     if(delta>0)//down
	//     {
	//       this.scaleFactor/=1.1;
	//     }else{//up
	//       this.scaleFactor*=1.1;
	//     }
	//     if(this.offsetX!=event.offsetX|| this.offsetY!=event.offsetY){
	//       this.offsetX=event.offsetX;
	//       this.offsetY=event.offsetY;
	//       //console.log("change X", this.offsetX);
	//       //console.log("change Y", this.offsetY);
	//     }
	//     var cx = this.offsetX , cy = this.offsetY;
	//     // var cx = 23* this.GridSize, cy = 16 * this.GridSize;
	//     //console.log("root X", cx);
	//     //console.log("root Y", cy);
	//     let X = this.scaleFactor*cx;
	//     let Y = this.scaleFactor*cy;
	//     this.panX = ( cx - X);
	//     this.panY = (cy - Y);
	//     this.ZoomPan();
	//     this.Draw();
	//     event.preventDefault();
	//     event.stopPropagation();
	// }
	//************************************** *//



	//#region ********************VERSION 2*******************************
	getNodeByID(ID) {
		let node = undefined;
		this.arrNode.forEach(element => {
			if (element.ID == ID) {
				node = element;
			}
		});
		return node;
	}
	UpdateTitleNode(selectedNode) {
		this.arrNode.forEach(element => {
			if (element.ID == selectedNode.ID) {
				element.Title = selectedNode.Title;
			}
		});
		this.Draw();
	}
	UpdateArrow(item, EndNode, IsComeBack = false) {
		try {
			this.arrArrow.forEach(element => {
				if (element.ID == item.ID) {
					element.End = EndNode.ID;
					element.IsComeBack = IsComeBack;
				}
			});
			this.Draw();
			this.selectedNode = this.getcontentRelate(item.Start);
			return this.selectedNode;
		} catch (Ex) {
			//console.log(Ex);
			return null;
		}
	}
	AddArrow(selectedNode, EndNode, IsComeBack = false) {
		try {
			let idx = this.arrArrow.length + 1;
			let arrow = {
				End: EndNode.ID,
				ID: idx,
				Start: selectedNode.ID,
				arrHead: false,
				IsComeBack: IsComeBack,
			}
			this.arrArrow.push(arrow);
			this.Draw();
			this.selectedNode = this.getcontentRelate(selectedNode.ID);
			return this.selectedNode;
		} catch (ex) {
			//console.log(ex);
			return null;
		}
	}

	DeleteArrow(item) {
		try {
			for (let i = 0; i < this.arrArrow.length; i++) {
				const element = this.arrArrow[i];
				if (element.ID == item.arrow.ID) {
					this.arrArrow.splice(i, 1);
				}
			}
			this.Draw();
			this.selectedNode = this.getcontentRelate(item.arrow.Start);
			return this.selectedNode;
		} catch (ex) {
			//console.log(ex);
			return null;
		}
	}

	setLevelNode() {
		this.arr_Level = [];
		if (this.arrArrow && this.arrArrow.length > 0) {
			let root = this.getNodeByID(this.arrArrow[0].Start);
			let arr_arrow = Object.assign([], this.arrArrow);
			let arr_nodes = Object.assign([], this.arrNode);
			if (root) {
				this.arr_Level[0] = [];
				this.arr_Level[0].push(root);
				let rindex = arr_nodes.findIndex(x => x.ID == root.ID);
				arr_nodes.splice(rindex, 1);
				for (var i = 0; i < this.arr_Level.length; i++) {
					for (var j = 0; j < this.arr_Level[i].length; j++) {
						for (var k = 0; k < arr_arrow.length; k++) {
							if (this.arr_Level[i][j] && arr_arrow[k].Start == this.arr_Level[i][j].ID ) {
								if(arr_arrow[k].Start != arr_arrow[k].End){
									let node = this.getNodeByID(arr_arrow[k].End);
									if (node) {
										let index = arr_nodes.findIndex(x => x.ID == node.ID);
										if (index > -1) {
											if (!this.arr_Level[i + 1]) this.arr_Level[i + 1] = [];
											this.arr_Level[i + 1].push(node);
											arr_nodes.splice(index, 1);
										}
										else {
											let idx = this.arr_Level[i].findIndex(x => x.ID == node.ID);//check có chung tầng không
											if (idx > -1) {
												if (!this.arr_Level[i + 1]) this.arr_Level[i + 1] = [];
												this.arr_Level[i + 1].push(node);
												this.arr_Level[i].splice(idx, 1);
											}
										}
									}
								}
								else{
									//console.log("arr_arrow queen", arr_arrow[k]);
								}
								
							}
						}
					}
				}
				if (arr_nodes.length > 0) {
					let final_idx = this.arr_Level.length;
					this.arr_Level[final_idx] = [];
					arr_nodes.forEach(element => {
						if (element) {
							this.arr_Level[final_idx].push(element);
						}
					});
				}
			}
		}
		else {
			let arr_nodes = Object.assign([], this.arrNode);
			if (arr_nodes.length > 0) {
				this.arr_Level[0] = [];
				arr_nodes.forEach(element => {
					if (element) {
						this.arr_Level[0].push(element);
					}
				});
			}
		}
	}
	//fill text and draw node
	drawLevelNode() {
		/// Vẽ nodes
		for (var i = 0; i < this.arr_Level.length; i++) {
			for (var j = 0; j < this.arr_Level[i].length; j++) {
				this.arr_Level[i][j].Level = i;
				this.arr_Level[i][j].Idx = j;
				let point_node = this.setLocationRectInLevel(i, j);
				if (point_node) {
					this.arr_Level[i][j].Location = point_node;
					this.wrapText(this.ctx, this.arr_Level[i][j].Title, (this.arr_Level[i][j].Location.x) * this.GridSize, (this.arr_Level[i][j].Location.y) * this.GridSize, this.widthRect * this.GridSize);
					this.rects.push(point_node);
				}
				this.arr_Level[i][j].arr_arrow = [];
			}
		}

		/// Xử lý lý dữ liệu arrows
		for (var i = 0; i < this.arr_Level.length; i++) {
			for (var j = 0; j < this.arr_Level[i].length; j++) {
				this.addArrowToNode(i, j);
			}
		}
		let lvlFakeLine = [];
		let lvlFakeLine_End = [];
		let lvlFakeCol = [];
		this.lines = [];

		//set index cho những node chung trục trước để không bị lấn vị trí ở giữa
		this.arr_arrowFake.forEach(arr => {
			let startNode = arr.startNode;
			let endNode = arr.endNode;
			if (startNode.Arr === undefined) startNode.Arr = [0, 0, 0, 0];
			if (endNode.Arr === undefined) endNode.Arr = [0, 0, 0, 0];
			let startPoint = this.setLocationRectInLevel(startNode.Level, startNode.Idx);
			let endPoint = this.setLocationRectInLevel(endNode.Level, endNode.Idx);
			if (endPoint.x == startPoint.x && (endNode.Level - startNode.Level) == 1) {
				arr.idx_start = startNode.Arr[1];
				startNode.Arr[1]++;
				arr.idx_end = endNode.Arr[3];
				endNode.Arr[3]++;
			}
			// if(arr.startNode.ID==arr.endNode.ID){
			// 	console.log('this.arr_arrowFake', arr);
			// 	arr.idx_start = startNode.Arr[1];
			// 	startNode.Arr[1]++;
			// 	arr.idx_end = endNode.Arr[1];
			// 	endNode.Arr[1]++;
			// }
		});
		//set index cho những vị trí còn lại, tính delta x, delta y cho arrow
		this.arr_arrowFake.forEach(arr => {
			let startNode = arr.startNode;
			let endNode = arr.endNode;
			if (startNode.Arr === undefined) startNode.Arr = [0, 0, 0, 0];
			if (endNode.Arr === undefined) endNode.Arr = [0, 0, 0, 0];
			if (lvlFakeLine[startNode.Level * 2] === undefined) lvlFakeLine[startNode.Level * 2] = 0;
			if (lvlFakeLine[startNode.Level * 2 + 1] === undefined) lvlFakeLine[startNode.Level * 2 + 1] = 0;
			if (lvlFakeLine_End[(endNode.Level - 1) * 2] === undefined) lvlFakeLine_End[(endNode.Level - 1) * 2] = 0;
			if (lvlFakeLine_End[endNode.Level * 2 - 1] === undefined) lvlFakeLine_End[endNode.Level * 2 - 1] = 1;
			if (lvlFakeCol[startNode.Idx] === undefined) lvlFakeCol[startNode.Idx] = 0;
			if (lvlFakeCol[startNode.Idx + 1] === undefined) lvlFakeCol[startNode.Idx + 1] = 0;
			let startPoint = this.setLocationRectInLevel(startNode.Level, startNode.Idx);
			let endPoint = this.setLocationRectInLevel(endNode.Level, endNode.Idx);
			arr.id_col = 0;
			arr.idx_col = 0;
			if (endPoint.x != startPoint.x || (endNode.Level - startNode.Level > 1)) {
				arr.idy_start = lvlFakeLine[startNode.Level * 2 + 1];
				lvlFakeLine[startNode.Level * 2 + 1]++;//tổng arrow start có start và end k chung trục
				if (endNode.Level - startNode.Level > 1) {
					//xét vị trí để tính delta col trung gian giữa các node chung level
					let _sign = 1;
					if (startNode.Idx % 2 == 0) {
						_sign = -1;
					}
					if (endPoint.x < startPoint.x) {
						if (startNode.Idx == 0) {
							_sign = 0;
						}
						lvlFakeCol[startNode.Idx + _sign]++;
						arr.idx_col = lvlFakeCol[startNode.Idx + _sign];
						arr.id_col = startNode.Idx + _sign;
					}
					else {
						lvlFakeCol[startNode.Idx - _sign]++;
						arr.idx_col = lvlFakeCol[startNode.Idx - _sign];
						arr.id_col = startNode.Idx - _sign;
					}
					//////
					//chỉ những arrow đi qua hơn 1 level mới tính delta y end cho nó
					arr.idy_end = lvlFakeLine_End[(endNode.Level) * 2 - 1];
					lvlFakeLine_End[(endNode.Level) * 2 - 1]++;//tổng arrow end có start và end k chung trục
				}
				if (this.checkNextNode(startNode, endNode, 0)) { //nếu các node kế node start trống thì đi ngang
					if (startPoint.x > endPoint.x) {
						arr.idx_start = startNode.Arr[0];
						startNode.Arr[0]++;
					}
					else if (startPoint.x < endPoint.x) {
						arr.idx_start = startNode.Arr[2];
						startNode.Arr[2]++;
					}
					else {
						arr.idx_start = startNode.Arr[1];
						startNode.Arr[1]++;
					}
					arr.idx_end = endNode.Arr[3];
					endNode.Arr[3]++;
				}
				else if (this.checkNextNode(startNode, endNode, 1)) {//edit
					if (startPoint.x > endPoint.x) {
						arr.idx_end = endNode.Arr[2];
						endNode.Arr[2]++;
					}
					else if (startPoint.x < endPoint.x) {
						arr.idx_end = endNode.Arr[0];
						endNode.Arr[0]++;
					}
					else {
						arr.idx_end = endNode.Arr[3];
						endNode.Arr[3]++;
					}
					arr.idx_start = startNode.Arr[1];
					startNode.Arr[1]++;
				}
				else {
					arr.idx_start = startNode.Arr[1];
					startNode.Arr[1]++;
					arr.idx_end = endNode.Arr[3];
					endNode.Arr[3]++;
				}
			}
			else {
				if (startNode == endNode) {
					// let _sign = 1;
					// if (startNode.Idx % 2 == 0) {
					// 	_sign = -1;
					// }
					// lvlFakeCol[startNode.Idx - _sign]++;
					// arr.idx_col = lvlFakeCol[startNode.Idx - _sign];
					// arr.id_col = startNode.Idx - _sign;
					arr.idx_start = startNode.Arr[1];
					startNode.Arr[1]++;
					arr.idx_end = endNode.Arr[3];
					endNode.Arr[3]++;
					//console.log('this.arr_arrowFake', arr);
				}
			}
		});
		let __calDeltaY = function (idx, total, arrow_h) {
			let _d = (idx / total) * arrow_h / 2;
			return _d;
		};
		let __calDeltaX = function (idx, total, widthRect) {
			let _d = (Math.ceil(idx / 2) / (total + 1)) * widthRect;
			return _d;
		};
		this.arr_arrowFake.forEach(arr => {
			let startNode = arr.startNode;
			let endNode = arr.endNode;
			let startPoint = this.setLocationRectInLevel(startNode.Level, startNode.Idx);
			let endPoint = this.setLocationRectInLevel(endNode.Level, endNode.Idx);
			let deltax = 0;
			if (arr.idx_start > 0) {
				if (arr.idx_start % 2 == 0) {
					deltax = __calDeltaX(arr.idx_start, startNode.Arr[1], this.widthRect);//Math.ceil(arr.idx_start/2)/startNode.Arr[0];//((arr.idx_start / 2) + 1) / 2;
				}
				else {
					deltax = -__calDeltaX(arr.idx_start, startNode.Arr[1], this.widthRect);//-Math.ceil(arr.idx_start/2)/startNode.Arr[0];//(-((arr.idx_start / 2) + 1)) / 2;
				}
			}
			let deltaxE = 0;
			if (arr.idx_end > 0) {
				if (arr.idx_end % 2 == 0) {
					deltaxE = __calDeltaX(arr.idx_end, endNode.Arr[3], this.widthRect);
				}
				else {
					deltaxE = -__calDeltaX(arr.idx_end, endNode.Arr[3], this.widthRect);
				}
			}
			let delta_mid = 0;
				if (arr.idx_start > 0 && arr.idx_col > 0) {
					if (arr.idx_start % 2 == 0) {
						delta_mid = arr.idx_col / lvlFakeCol[arr.id_col];
					}
					else {
						delta_mid = -arr.idx_col / lvlFakeCol[arr.id_col];
					}
				}
	
				let deltay = 0;
				if (arr.idy_start > 0) {
					deltay = __calDeltaY(arr.idy_start, lvlFakeLine[startNode.Level * 2 + 1], this.arrow_h);
				}
				let deltayE = 0;
				if (arr.idy_end > 0) {
					deltayE = __calDeltaY(arr.idy_end, lvlFakeLine_End[(endNode.Level) * 2 - 1], this.arrow_h);
				}
			// if(startNode!=endNode){
				//#region  set Delta 
	
				
				//#endregion end set Delta
				if (endPoint.x == startPoint.x) { //chung trên một trục
					let line = {};
					if (endNode.Level - startNode.Level > 1) {
						let lines = [this.arrow_h / 2 - deltay,//+ mid_level ,//this.arrow_h - 2*deltay
						(this.widthRect) - deltax - delta_mid,
						(endPoint.y - startPoint.y) - (this.heightRect + this.arrow_h) + deltay + deltayE,//  - deltayE ,// - mid_level ,
						endPoint.x - startPoint.x - (this.widthRect) + deltaxE + delta_mid,
						this.arrow_h / 2 - deltayE]//2 * deltay];
	
						if (startNode.Level + 1 == endNode.Level) {
							lines[4] += lines[2];
							lines[2] = 0;
						}
						line = {
							startPos: { x: deltax + startPoint.x + (this.widthRect / 2), y: startPoint.y + this.heightRect, d: false, arrHead: arr.arrHead, IsComeBack: arr.IsComeBack },
							lines: lines
						};
					}
					else {
						if(endNode.Level==startNode.Level){
							//console.log("deltax", deltax);
							//console.log("startPoint", startPoint);
							//console.log("deltay", deltay);
							//console.log("delta_mid", delta_mid);
							line = {
								startPos: { x: deltax + startPoint.x + this.widthRect / 2, y: startPoint.y + this.heightRect, d: false, arrHead: arr.arrHead,  IsComeBack: arr.IsComeBack  },
								lines: [1-deltax,(deltax*2 + this.widthRect / 2),-1+deltax]
							};
						}
						else{
							line = {
								startPos: { x: deltax + startPoint.x + this.widthRect / 2, y: startPoint.y + this.heightRect, d: false, arrHead: arr.arrHead,  IsComeBack: arr.IsComeBack  },
								lines: [endPoint.y - startPoint.y - this.heightRect]
							};
						}						
					}
	
					this.lines.push(line);
				} else if (endPoint.x > startPoint.x) {//sang phải
					let line = {};
					if (this.checkNextNode(startNode, endNode, 0)) {//trường hợp đặc biệt đi ngang nếu k có node kề bên
						let deltanext_y = 0;
						if (arr.idx_start > 0) {
							if (arr.idx_start % 2 == 0) {
								deltanext_y = __calDeltaX(arr.idx_start, startNode.Arr[2], this.heightRect);//Math.ceil(arr.idx_start/2)/startNode.Arr[0];//((arr.idx_start / 2) + 1) / 2;
							}
							else {
								deltanext_y = -__calDeltaX(arr.idx_start, startNode.Arr[2], this.heightRect);//-Math.ceil(arr.idx_start/2)/startNode.Arr[0];//(-((arr.idx_start / 2) + 1)) / 2;
							}
						}
	
						line = {
							startPos: { x: startPoint.x + this.widthRect, y: deltanext_y + startPoint.y + this.heightRect / 2, d: true, arrHead: arr.arrHead,  IsComeBack: arr.IsComeBack  },
							lines: [endPoint.x - startPoint.x - (this.widthRect / 2) - deltaxE, (endPoint.y - startPoint.y - this.heightRect / 2) - deltanext_y]
						};
					}
					else if (this.checkNextNode(startNode, endNode, 1)) {
						let deltanext_y = 0;
						if (arr.idx_end > 0) {
							if (arr.idx_end % 2 == 0) {
								deltanext_y = __calDeltaX(arr.idx_end, endNode.Arr[0], this.heightRect);
							}
							else {
								deltanext_y = -__calDeltaX(arr.idx_end, endNode.Arr[0], this.heightRect);
							}
						}
	
						line = {
							startPos: { x: startPoint.x + this.widthRect / 2 + deltax, y: startPoint.y + this.heightRect, d: false, arrHead: arr.arrHead , IsComeBack: arr.IsComeBack },
							lines: [endPoint.y - startPoint.y - this.heightRect / 2 - deltanext_y,
							endPoint.x - startPoint.x - this.widthRect / 2 - deltax]
						};
					}
					else {
						let lines = [this.arrow_h / 2 - deltay,//+ mid_level ,//this.arrow_h - 2*deltay
						(this.widthRect) - deltax - delta_mid,
						(endPoint.y - startPoint.y) - (this.heightRect + this.arrow_h) + deltay + deltayE,//  - deltayE ,// - mid_level ,
						endPoint.x - startPoint.x - (this.widthRect) + deltaxE + delta_mid,
						this.arrow_h / 2 - deltayE]//2 * deltay];
	
						if (startNode.Level + 1 == endNode.Level) {
							lines[4] += lines[2];
							lines[2] = 0;
						}
						line = {
							startPos: { x: deltax + startPoint.x + (this.widthRect / 2), y: startPoint.y + this.heightRect, d: false, arrHead: arr.arrHead, IsComeBack: arr.IsComeBack  },
							lines: lines
						};
					}
					this.lines.push(line);
				} else {//sang trái
					let line = {};
					if (this.checkNextNode(startNode, endNode, 0)) { //trường hợp đặc biệt đi ngang nếu k có node kề bên
						let deltanext_y = 0;
						if (arr.idx_start > 0) {
							if (arr.idx_start % 2 == 0) {
								deltanext_y = __calDeltaX(arr.idx_start, startNode.Arr[0], this.heightRect);//Math.ceil(arr.idx_start/2)/startNode.Arr[0];//((arr.idx_start / 2) + 1) / 2;
							}
							else {
								deltanext_y = -__calDeltaX(arr.idx_start, startNode.Arr[0], this.heightRect);//-Math.ceil(arr.idx_start/2)/startNode.Arr[0];//(-((arr.idx_start / 2) + 1)) / 2;
							}
						}
	
						line = {
							startPos: { x: startPoint.x, y: deltanext_y + startPoint.y + this.heightRect / 2, d: true, arrHead: arr.arrHead, IsComeBack: arr.IsComeBack  },
							lines: [endPoint.x - startPoint.x + (this.widthRect / 2) - deltaxE, (endPoint.y - startPoint.y - this.heightRect / 2) - deltanext_y]
						};
					}
					else if (this.checkNextNode(startNode, endNode, 1)) {
						let deltanext_y = 0;
						if (arr.idx_end > 0) {
							if (arr.idx_end % 2 == 0) {
								deltanext_y = __calDeltaX(arr.idx_end, endNode.Arr[2], this.heightRect);
							}
							else {
								deltanext_y = -__calDeltaX(arr.idx_end, endNode.Arr[2], this.heightRect);
							}
						}
	
						line = {
							startPos: { x: startPoint.x + this.widthRect / 2 - deltax, y: startPoint.y + this.heightRect, d: false, arrHead: arr.arrHead, IsComeBack: arr.IsComeBack  },
							lines: [endPoint.y - startPoint.y - this.heightRect / 2 - deltanext_y,
							endPoint.x - startPoint.x + this.widthRect / 2 + deltax]
						};
					}
					else {
						let lines = [this.arrow_h / 2 - deltay,// + mid_level,//this.arrow_h - 2 * deltay,
						-this.widthRect - deltax - delta_mid,
						(endPoint.y - startPoint.y) - (this.heightRect + this.arrow_h) + deltay + deltayE,//- deltayE ,// - mid_level,
						endPoint.x - startPoint.x + (this.widthRect) + deltaxE + delta_mid,
						this.arrow_h / 2 - deltayE]// 2 * deltay]
						if (startNode.Level + 1 == endNode.Level) {
							lines[4] += lines[2];
							lines[2] = 0;
						}
						line = {
							startPos: { x: deltax + startPoint.x + (this.widthRect / 2), y: startPoint.y + this.heightRect, d: false, arrHead: arr.arrHead , IsComeBack: arr.IsComeBack },
							lines: lines
						};
					}
					this.lines.push(line);
				}
			// }
			// else{
			// 	console.log('this.arr_arrowFake 2', arr);
			// 	console.log('startPoint 2', startPoint);
			// 	let line={
			// 		startPos: {x:startPoint.x, y: this.heightRect - startPoint.x, d: false, arrHead: arr.arrHead , IsComeBack: arr.IsComeBack }, 
			// 		lines:[- 1, 1,1]
			// 	}
			// 	console.log('line2', line);
			// 	this.lines.push(line);
			// }
			
		});
	}
	drawLevelNode1() {
		/// Vẽ nodes
		for (var i = 0; i < this.arr_Level.length; i++) {
			for (var j = 0; j < this.arr_Level[i].length; j++) {
				this.arr_Level[i][j].Level = i;
				this.arr_Level[i][j].Idx = j;
				this.arr_Level[i][j].Relative = [0, 0, 0, 0];
				let point_node = this.setLocationRectInLevel(i, j);
				if (point_node) {
					this.arr_Level[i][j].Location = point_node;
					this.ctx.fillText(this.arr_Level[i][j].Title, (this.arr_Level[i][j].Location.x + 0.5) * this.GridSize, (this.arr_Level[i][j].Location.y + 1) * this.GridSize); //viết text
					this.rects.push(point_node);
				}
				this.arr_Level[i][j].arr_arrow = [];
			}
		}

		/// Xử lý lý dữ liệu arrows
		for (var i = 0; i < this.arr_Level.length; i++) {
			for (var j = 0; j < this.arr_Level[i].length; j++) {
				this.addArrowToNode(i, j);
			}
		}
		let lvlLine = [];
		this.lines = [];
		//console.log('this.arr_arrowFake', this.arr_arrowFake);
		this.arr_arrowFake.forEach(arr => {
			let startNode = arr.startNode;
			let endNode = arr.endNode;

			let startPoint = this.setLocationRectInLevel(startNode.Level, startNode.Idx);
			let endPoint = this.setLocationRectInLevel(endNode.Level, endNode.Idx);

			if (startNode.Arr === undefined) startNode.Arr = [0, 0, 0, 0];
			if (endNode.Arr === undefined) endNode.Arr = [0, 0, 0, 0];
			if (lvlLine[startNode.Level * 2] === undefined) lvlLine[startNode.Level * 2] = 0;
			if (lvlLine[startNode.Level * 2 + 1] === undefined) lvlLine[startNode.Level * 2 + 1] = 0;//1
			if (endPoint.x == startPoint.x) { //chung trên một trục

				let deltax = 0;
				if (startNode.Arr[1] > 0) {
					if (startNode.Arr[1] % 2 == 0) {
						deltax = ((startNode.Arr[1] / 2) + 1) / 2;
					}
					else {
						deltax = (-((startNode.Arr[1] / 2) + 1)) / 2;
					}
				}
				let line = {
					startPos: { x: deltax + startPoint.x + this.widthRect / 2, y: startPoint.y + this.heightRect, d: false, arrHead: arr.arrHead, IsComeBack: arr.IsComeBack  },
					lines: [endPoint.y - startPoint.y - this.heightRect]
				};
				this.lines.push(line);
				startNode.Arr[1]++;
				endNode.Arr[3]++;
			} else if (endPoint.x > startPoint.x) {//sang phải
				let deltax = 0;
				if (startNode.Arr[1] > 0) {
					if (startNode.Arr[1] % 2 == 0) {
						deltax = ((startNode.Arr[1] / 2) + 1) / 2;
					}
					else {
						deltax = (-((startNode.Arr[1] / 2) + 1)) / 2;
					}
				}

				let deltaxE = 0;
				if (endNode.Arr[3] > 0) {
					if (endNode.Arr[3] % 2 == 0) {
						deltaxE = ((endNode.Arr[3] / 2) + 1) / 2;
					}
					else {
						deltaxE = (-((endNode.Arr[3] / 2) + 1)) / 2;
					}
				}

				let deltay = 0;
				if (lvlLine[startNode.Level * 2 + 1] > 0) {
					deltay = lvlLine[startNode.Level * 2 + 1] / 4;
				}
				startNode.Arr[1]++;
				endNode.Arr[3]++;
				lvlLine[startNode.Level * 2 + 1]++;
				let line = {
					startPos: { x: deltax + startPoint.x + (this.widthRect / 2), y: startPoint.y + this.heightRect, d: false, arrHead: arr.arrHead, IsComeBack: arr.IsComeBack  },
					lines: [this.arrow_h / 2 - deltay,
					(this.widthRect) - deltax,
					(endPoint.y - startPoint.y) - (this.heightRect + this.arrow_h),
					endPoint.x - startPoint.x - (this.widthRect) + deltaxE,
					this.arrow_h / 2 + deltay]
				};
				this.lines.push(line);
			} else {//sang trái

				let deltax = 0;
				if (startNode.Arr[1] > 0) {
					if (startNode.Arr[1] % 2 == 0) {
						deltax = ((startNode.Arr[1] / 2) + 1) / 2;
					}
					else {
						deltax = (-((startNode.Arr[1] / 2) + 1)) / 2;
					}
				}
				let deltay = 0;
				if (lvlLine[startNode.Level * 2 + 1] > 0) {
					deltay = lvlLine[startNode.Level * 2 + 1] / 4;
				}

				let deltaxE = 0;
				if (endNode.Arr[3] > 0) {
					if (endNode.Arr[3] % 2 == 0) {
						deltaxE = ((endNode.Arr[3] / 2) + 1) / 2;
					}
					else {
						deltaxE = (-((endNode.Arr[3] / 2) + 1)) / 2;
					}
				}
				startNode.Arr[1]++;
				endNode.Arr[3]++;

				lvlLine[startNode.Level * 2 + 1]++;

				let line = {
					startPos: { x: deltax + startPoint.x + (this.widthRect / 2), y: startPoint.y + this.heightRect, d: false, arrHead: arr.arrHead, IsComeBack: arr.IsComeBack  },
					lines: [this.arrow_h / 2 - deltay,
					-this.widthRect - deltax,
					(endPoint.y - startPoint.y) - (this.heightRect + this.arrow_h),
					endPoint.x - startPoint.x + (this.widthRect) + deltaxE,
					this.arrow_h / 2 + deltay]
				};
				this.lines.push(line);
			}
		});
	}

	addArrowToNode(level, idx) {
		let pos_start = 0;
		let pos_end = 0;
		var node = this.arr_Level[level][idx];
		if (node) {
			this.arr_arrowFake.forEach(arr => {
				let item_arrow = {
					ID: arr.ID,
					Start: arr.Start,
					End: arr.End,
					Level: level,
					Idx_Start: -1,
					Idx_End: -1,
					Idx_node: idx,
					Index: -1
				}
				if (arr.Start == node.ID) {
					let nodeEnd = this.getNodeInLevelArray(arr.End);
					if (nodeEnd) {
						if (nodeEnd.Level < node.Level) {
							arr.startNode = nodeEnd;
							arr.endNode = node;
							arr.arrHead = true;
						}
						else {
							arr.startNode = node;
							arr.endNode = nodeEnd;
							arr.arrHead = false;
						}
						arr.endNode.Arr = undefined;
						arr.startNode.Arr = undefined;
						if (!node.arr_arrow) node.arr_arrow = [];
						item_arrow.Index = node.arr_arrow.length;
						node.arr_arrow.push(item_arrow);
					}
				}
			});
		}
	}


	//set vị trí dàn canh theo trái phải của mỗi item trên level
	getNodeInLevelArray(Id) {
		let node = undefined;
		for (var i = 0; i < this.arr_Level.length; i++) {
			for (var j = 0; j < this.arr_Level[i].length; j++) {
				if (this.arr_Level[i][j].ID == Id) {
					node = this.arr_Level[i][j];
				}
			}
		}
		return node;
	}
	checkNextNode(startNode, endNode, type) {
		//kiểm tra số lượng node đầu và node cuối để biết đi từ node ít đến node nhiều hay ngược lại
		if (type == 0) {// trường hợp start < end  
			if (this.arr_Level[startNode.Level].length > this.arr_Level[endNode.Level].length) {
				return false;
			}
			if (this.arr_Level[startNode.Level][endNode.Idx]) {//kiểm tra ngay trên đầu node end 
				return false;
			}
			if (startNode.Idx % 2 == 0) {
				if (startNode == 0) {
					if (endNode.Idx % 2 == 0) {
						if (this.arr_Level[startNode.Level][startNode.Idx + 2]) { //kế bên node start cùng phía
							return false;
						}
					}
					else {
						if (this.arr_Level[startNode.Level][startNode.Idx + 1]) { //kế bên node start khác phía
							return false;
						}
					}
				}
				else {
					if (endNode.Idx % 2 == 0) {
						if (this.arr_Level[startNode.Level][startNode.Idx + 2] || this.arr_Level[startNode.Level][startNode.Idx + 1]) { //kế bên node start cùng phía
							return false;
						}
					}
					else {
						if (this.arr_Level[startNode.Level][startNode.Idx - 2] || this.arr_Level[startNode.Level][startNode.Idx - 1]) { //kế bên node start khác phía
							return false;
						}
					}
				}

			}
			else {
				if (endNode.Idx % 2 != 0) {
					if (this.arr_Level[startNode.Level][startNode.Idx + 2] || this.arr_Level[startNode.Level][startNode.Idx + 1]) { //kế bên node start cùng phía
						return false;
					}
				}
				else {
					if (this.arr_Level[startNode.Level][startNode.Idx - 2] || this.arr_Level[startNode.Level][startNode.Idx - 1]) { //kế bên node start khác phía
						return false;
					}
				}

			}

		}
		else if (type == 1) {//trường hợp start > end
			if (this.arr_Level[startNode.Level].length < this.arr_Level[endNode.Level].length) {
				return false;
			}
			if (this.arr_Level[endNode.Level][startNode.Idx]) {//kiểm tra ngay trên đầu node start
				return false;
			}
			if (endNode.Idx % 2 == 0) {
				if (endNode.Idx == 0) {
					if (startNode.Idx % 2 == 0) {
						if (this.arr_Level[endNode.Level][endNode.Idx + 2]) {//kế bên node end cùng phía
							return false;
						}
					}
					else {
						if (this.arr_Level[endNode.Level][endNode.Idx + 1]) {//kế bên node end khác phía
							return false;
						}
					}
				}
				else {
					if (startNode.Idx % 2 == 0) {
						if (this.arr_Level[endNode.Level][endNode.Idx + 2] || this.arr_Level[endNode.Level][endNode.Idx + 1]) {//kế bên node end cùng phía
							return false;
						}
					}
					else {
						if (this.arr_Level[endNode.Level][endNode.Idx - 2] || this.arr_Level[endNode.Level][endNode.Idx - 1]) {//kế bên node end khác phía
							return false;
						}
					}
				}
			} else {
				if (startNode.Idx % 2 != 0) {
					if (this.arr_Level[endNode.Level][endNode.Idx + 2] || this.arr_Level[endNode.Level][endNode.Idx + 1]) {//kế bên node end cùng phía
						return false;
					}
				}
				else {
					if (this.arr_Level[endNode.Level][endNode.Idx - 2] || this.arr_Level[endNode.Level][endNode.Idx - 1]) {//kế bên node end khác phía 
						return false;
					}
				}
			}
		}
		  
		for (var i = startNode.Level + 1; i < endNode.Level; i++) {
			if (type == 0) {//trường hợp đi ngang từ trên xuống 0->3, 2->3 :trường hợp start < end   
				if (this.arr_Level[i][endNode.Idx]) {//kiểm tra trên đầu node end
					return false;
				}
			}
			else if (type == 1) {  //  trường hợp đi ngang từ trên xuống 1->0, 1->2 :trường hợp start > end                     
				if (this.arr_Level[i][startNode.Idx]) {//kiểm tra trên đầu node start
					return false;
				}
			}
		}
		return true;
	}

	setLocationRectInLevel(level, idx) {
		let point_node;
		let x_lc = (this.canvas.width / this.GridSize) / 2;// + idx * (2 * this.widthRect); idx == 0
		if (idx > 0) {
			if (idx % 2 == 1) {//this.isLeft) {

				x_lc = (this.canvas.width / this.GridSize) / 2 - (2 * this.widthRect) * ((idx + 1) / 2);//this.cr_left;
			}
			else {
				x_lc = (this.canvas.width / this.GridSize) / 2 + (2 * this.widthRect) * (idx / 2);//this.cr_right;
			}
			this.isLeft = !this.isLeft;
		}
		point_node = {
			x: x_lc,
			y: (this.heightRect + this.arrow_h) * level + 2,
			w: this.widthRect,
			h: this.heightRect,
			c: this.arr_Level[level][idx].BorderColor,
			b: this.arr_Level[level][idx].BackgroundColor,
			tip: this.arr_Level[level][idx].Title,
			ID: this.arr_Level[level][idx].ID,
			isleft: idx % 2 ? true : false// !this.isLeft
		};
		return point_node;
	}


	OutputStringFakeArray(array) {
		let str = '\r\n';
		for (var i = 0; i < array.length; i++) {

			for (var j = 0; j < array[i].length; j++) {
				str += array[i][j] + ', ';//'['+i+','+j+']'+
			}
			str += '\r\n';
		}
		return str;
	}
	//#endregion
}
