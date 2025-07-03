<template>
  <div v-if="message" style="color:green">
    新政策推送：{{ message }}
  </div>
</template>
<script setup>
  import { ref, onMounted } from 'vue'
  import * as signalR from '@microsoft/signalr'

  const message = ref('')
  onMounted(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl('/policyhub')
      .build()
    connection.on('ReceivePolicy', msg => {
      message.value = msg
    })
    connection.start()
  })
</script>

