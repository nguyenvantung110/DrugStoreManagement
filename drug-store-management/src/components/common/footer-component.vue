<template lang="">
    <v-footer class="d-flex flex-column flex-0-0" color="teal" rounded="lg">
        <div class="d-flex w-100 align-center px-4 py-2">
        <strong>Get connected with us on social networks!</strong>

        <div class="d-flex ga-2 ms-auto">
            <v-btn
            v-for="icon in icons"
            :key="icon"
            :icon="icon"
            size="small"
            variant="plain"
            ></v-btn>
        </div>
        </div>

        <div class="px-4 py-2 bg-surface-variant text-center w-100 rounded-lg">
        {{ new Date().getFullYear() }} — <strong>TungNV</strong>
        </div>
    </v-footer>
</template>
<script setup lang="ts"></script>
  