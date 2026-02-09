import { useState, useEffect } from 'react'

import './App.css'

function App() {
  const [webCamIds, setWebCamIds] = useState<MediaDeviceInfo[]>([]);
  useEffect(() => {
    const init = async () => {
      const supported = navigator.mediaDevices.getSupportedConstraints();
  
  console.log(supported);
      await navigator.mediaDevices.getUserMedia({ video: true });
      setWebCamIds((await navigator.mediaDevices.enumerateDevices()).filter(device => device.kind === 'videoinput'));
      console.log(navigator.mediaDevices.getSupportedConstraints());
    }
    init();
  }, []);

  const readWebCam = async (id: ConstrainDOMString) => {
    const stream = await navigator.mediaDevices.getUserMedia({ video: { deviceId: id } });
    stream.getVideoTracks().forEach(track => {
      console.log('Track settings:', track.getSettings());
      console.log('Track getCapabilities:', track.getCapabilities());
    });
    // const video = document.createElement('video');
    // video.srcObject = stream;
    // video.play();
    // document.body.appendChild(video);
  }
  return (
    <>
      <div>
        <p>WebCam Devices:</p>
        <select onChange={x=>readWebCam(x.target.value)}>
          {webCamIds.map((device) => (
            <option key={device.deviceId} value={device.deviceId}>
              {device.label || 'Unknown Device'}
            </option>
          ))}
        </select>
      </div>
    </>
  )
}

export default App
